using System;
using DuskTillDawn.Core;
using Godot;

public partial class Main : Node3D
{
	private readonly GameState _state = new();
	private Hud _hud = null!;
	private const double SunFadeDegrees = 10;
	private const double SunMinElevationDegrees = 5;
	private const double ShadowFadeDegrees = 10;
	private const double SunWhiteFromDegrees = 30;
	private const double DiskFadeDegrees = 1;

	private const float MoonDistance = 900f;

	// Tunable in the Inspector (select the Main node). The daytime values live on the nodes themselves:
	// the lights' Energy and Color, and the WorldEnvironment's sky colours and energies.
	[ExportGroup("Sunrise & Sunset")]
	[Export] public Color SunHorizonColor { get; set; } = new(1f, 0.55f, 0.25f);

	// Night lighting: dark, but still readable.
	[ExportGroup("Night")]
	[Export] public Color NightSkyTopColor { get; set; } = new(0.01f, 0.015f, 0.05f);
	[Export] public Color NightSkyHorizonColor { get; set; } = new(0.03f, 0.04f, 0.1f);
	[Export(PropertyHint.Range, "0,1,0.01")] public float NightSkyFraction { get; set; } = 0.05f;
	[Export] public Color NightAmbientColor { get; set; } = new(0.25f, 0.3f, 0.45f);
	[Export(PropertyHint.Range, "0,4,0.05")] public float NightAmbientEnergy { get; set; } = 0.6f;

	// How much of the sky the moon and the sun's disk cover, in degrees (the real ones are about 0.5).
	[ExportGroup("Moon")]
	[Export(PropertyHint.Range, "0.5,45,0.5")] public float MoonSizeDegrees { get; set; } = 12f;

	private DirectionalLight3D _sun = null!;
	private float _sunEnergy;
	private DirectionalLight3D _moon = null!;
	private float _moonEnergy;
	private Node3D _moonBody = null!;
	private Godot.Environment _environment = null!;
	private float _dayAmbientEnergy;
	private float _daySkyEnergy;
	private ShaderMaterial _skyMaterial = null!;
	private Color _daySkyTopColor;
	private Color _daySkyHorizonColor;

	public override void _Ready()
	{
		_hud = GetNode<Hud>("Hud");
		_sun = GetNode<DirectionalLight3D>("DirectionalLight3D");
		_sunEnergy = _sun.LightEnergy;
		_moon = GetNode<DirectionalLight3D>("MoonLight3D");
		_moonEnergy = _moon.LightEnergy;
		_moonBody = GetNode<Node3D>("Moon");
		_environment = GetNode<WorldEnvironment>("WorldEnvironment").Environment;
		_dayAmbientEnergy = _environment.AmbientLightEnergy;
		_daySkyEnergy = _environment.BackgroundEnergyMultiplier;
		_environment.AmbientLightColor = NightAmbientColor;
		_skyMaterial = (ShaderMaterial)_environment.Sky.SkyMaterial;
		_daySkyTopColor = _skyMaterial.GetShaderParameter("sky_top_color").AsColor();
		_daySkyHorizonColor = _skyMaterial.GetShaderParameter("sky_horizon_color").AsColor();
		UpdateSun();
		UpdateMoon();
		ShowStateAndPauseIfGameEnded();
		AddMissingStairCollision(this);
	}

	// The imported buildings ship without collision on some "Stairs_*" meshes.
	private static void AddMissingStairCollision(Node node)
	{
		foreach (Node child in node.GetChildren())
		{
			if (child is MeshInstance3D { Mesh: not null } mesh
				&& mesh.Name.ToString().StartsWith("Stairs_")
				&& mesh.GetNodeOrNull<StaticBody3D>("StaticBody3D") == null)
			{
				mesh.CreateConvexCollision(); // hull of a staircase = a smooth ramp
			}

			AddMissingStairCollision(child);
		}
	}

	public override void _Process(double delta)
	{
		_state.Tick(delta);
		UpdateSun();
		UpdateMoon();
		ShowStateAndPauseIfGameEnded();
	}

	private void UpdateSun()
	{
		var sun = SunPosition.At(_state.HourOfDay);

		// Very flat light gives stretched, flickering shadows, so keep the beam a bit above the horizon.
		double pitch = sun.IsUp ? Math.Max(sun.ElevationDegrees, SunMinElevationDegrees) : sun.ElevationDegrees;
		_sun.RotationDegrees = new Vector3((float)-pitch, (float)-sun.AzimuthDegrees, 0);

		// Fade in over the first degrees after sunrise (and out before sunset) instead of popping.
		double fade = sun.IsUp ? Math.Clamp(sun.ElevationDegrees / SunFadeDegrees, 0, 1) : 0;
		_sun.LightEnergy = (float)(_sunEnergy * fade);
		SetShadowStrength(_sun, sun.IsUp, sun.ElevationDegrees);

		// Warm orange near the horizon, neutral white once the sun is high.
		float whiteness = (float)Math.Clamp(sun.ElevationDegrees / SunWhiteFromDegrees, 0, 1);
		_sun.LightColor = SunHorizonColor.Lerp(Colors.White, whiteness);

		// Sky: dark at night, blue by day, with an orange horizon while the sun is low.
		Color dayHorizon = SunHorizonColor.Lerp(_daySkyHorizonColor, whiteness);
		_skyMaterial.SetShaderParameter("sky_top_color", NightSkyTopColor.Lerp(_daySkyTopColor, (float)fade));
		_skyMaterial.SetShaderParameter("sky_horizon_color", NightSkyHorizonColor.Lerp(dayHorizon, (float)fade));

		// Sun disk at its real position (not the clamped beam), stars come out as the daylight fades.
		_skyMaterial.SetShaderParameter("sun_direction", SkyDirection(sun.AzimuthDegrees, sun.ElevationDegrees));
		_skyMaterial.SetShaderParameter("sun_color", _sun.LightColor);
		_skyMaterial.SetShaderParameter("sun_size_degrees", MoonSizeDegrees);
		_skyMaterial.SetShaderParameter("sun_visibility", DiskVisibility(sun.IsUp, sun.ElevationDegrees));
		_skyMaterial.SetShaderParameter("star_visibility", 1f - (float)fade);

		// By day the ambient light comes from the sky. At night the sky is almost black, so it blends
		// to a fixed moonlit blue instead: dark, but bright enough to see where you are going.
		_environment.AmbientLightSkyContribution = (float)fade;
		_environment.AmbientLightEnergy = Mathf.Lerp(NightAmbientEnergy, _dayAmbientEnergy, (float)fade);
		_environment.BackgroundEnergyMultiplier = _daySkyEnergy * Mathf.Lerp(NightSkyFraction, 1f, (float)fade);
	}

	// Same axes as the sun. The pitch is always negative, so the moon only ever shines down on the scene.
	private void UpdateMoon()
	{
		var moon = MoonPosition.At(_state.HourOfDay);

		double pitch = Math.Max(moon.ElevationDegrees, SunMinElevationDegrees);
		_moon.RotationDegrees = new Vector3((float)-pitch, (float)-moon.AzimuthDegrees, 0);

		double fade = moon.IsUp ? Math.Clamp(moon.ElevationDegrees / SunFadeDegrees, 0, 1) : 0;
		_moon.LightEnergy = (float)(_moonEnergy * fade);
		SetShadowStrength(_moon, moon.IsUp, moon.ElevationDegrees);

		PlaceMoonBody(moon.IsUp, moon.AzimuthDegrees, moon.ElevationDegrees);
	}

	// The visible moon follows the camera at a fixed distance, so it never gets closer as the player walks
	private void PlaceMoonBody(bool isUp, double azimuthDegrees, double elevationDegrees)
	{
		Camera3D? camera = GetViewport().GetCamera3D();
		_moonBody.Visible = isUp && camera != null;
		if (!_moonBody.Visible)
		{
			return;
		}

		Vector3 direction = SkyDirection(azimuthDegrees, elevationDegrees);
		float diameter = 2f * MoonDistance * Mathf.Tan(Mathf.DegToRad(MoonSizeDegrees) / 2f);
		Basis facingPlayer = Basis.LookingAt(direction, Vector3.Up).Scaled(Vector3.One * diameter);
		_moonBody.GlobalTransform = new Transform3D(facingPlayer, camera!.GlobalPosition + direction * MoonDistance);
	}

	// Direction from the scene towards the sun or moon, using the same axes as the lights
	private static Vector3 SkyDirection(double azimuthDegrees, double elevationDegrees)
	{
		var rotation = new Vector3(Mathf.DegToRad((float)-elevationDegrees), Mathf.DegToRad((float)-azimuthDegrees), 0);
		return Basis.FromEuler(rotation) * Vector3.Back;
	}

	// The sun disk fades over the last degree before they touch the horizon, so they never pop.
	private static float DiskVisibility(bool isUp, double elevationDegrees)
	{
		return isUp ? (float)Math.Clamp(elevationDegrees / DiskFadeDegrees, 0, 1) : 0f;
	}

	// Very flat light makes long, flickering shadows (and below the minimum elevation the beam is clamped,
	// so shadows would point the wrong way). Shadows start just above the minimum and fade in over a few degrees.
	private static void SetShadowStrength(DirectionalLight3D light, bool isUp, double elevationDegrees)
	{
		double strength = isUp ? Math.Clamp((elevationDegrees - SunMinElevationDegrees) / ShadowFadeDegrees, 0, 1) : 0;
		light.ShadowEnabled = strength > 0;
		light.ShadowOpacity = (float)strength;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!OS.IsDebugBuild() || @event is not InputEventKey { Pressed: true } key)
		{
			return;
		}

		if (key.Keycode == Key.F9)
		{
			_state.DrainSla(_state.Sla);
		}
		else if (key.Keycode == Key.F10)
		{
			_state.Tick(double.MaxValue);
		}

		ShowStateAndPauseIfGameEnded();
	}

	private void ShowStateAndPauseIfGameEnded()
	{
		_hud.ShowState(_state);
		GetTree().Paused = _state.Status == NightStatus.Lost || _state.DaySurvived;
	}
}
