namespace DuskTillDawn.Core;

// Done, nothing to code. What the Monitoring app will show for a server.
// Ok: no incident on it. Down: a running incident targets it.
// Warn: not used yet; meant for a server in danger (hot, overloaded) once Heat (#101) lands.
public enum ServerStatus
{
    Ok,
    Warn,
    Down,
}
