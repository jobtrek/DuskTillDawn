### Wednesday plan

For the plan on Wednesday, I think we first need to organize on what are the core aspects of the game and learn the basics of Godot engine and game engines in general.

### 3D models benchmark

For the 3D models, I need to run the benchmark and figure out an accurate algorithm to find the best model that's token efficient and produces the best models in voxel pixels.

The criteria for that:
- Token cost per model
- Accuracy and details of the spec
- Take into account the aspects of the Godot engine and how it renders 3D models
- Test between all models

### Godot basics

For the basics of Godot engine, I believe it's better if one person learns the basics, then explains it to the other teammates of the group, so that everyone has the same base that we could build on top of.

### Project organisation

For the project organisation, the methodologies are the same (Scrum). However, one thing that has changed is that this entire project will be made in OOP C#.

### Define the core contract for the game

We need to decide, all 3 of us, on what will be the core elements and mechanics of the first version of the game. This file will be called `contract.md`, written by hand across all our ideas and suggestions, and also the technical decisions, like if the game is in 2D or in 3D.

### Define the Scrum roles and responsibilities

Discuss with Bilal on who is the PO and who is the Scrum Master.

### Things we need to be careful about since the last 3 project groups

- Split the tasks vertically and make sure the tasks are vertically sliced
- Decide a unanimous contract or ADR on the technical details, the user stories and the core game aspects of the game to avoid confusion and 3 different ways of doing something
- Everyone should start from the same base, and not one person with a lot more knowledge on one thing and then everyone else falls behind
- Less AI usage to avoid AI bloat and review fatigue when reviewing code or documents
- Minimize the documentation made by AI and do pair reviews for ADRs and contracts decided
- Ensure good communication practices and make small points to decide implementation details or best practices
    - Godot best practices: https://docs.godotengine.org/en/stable/tutorials/best_practices/index.html
    - C# best practices: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
- Making sure, when grilling with Claude or any other agent, to *understand the question being asked and not accept everything*
- If generating documentation with Claude, *make sure it's understood by everyone and agreed upon and not 500 lines of full bullshit*
- When reviewing code or documents, *NOT TO SKIM THROUGH THE CODE BRIEFLY AND APPROVE BECAUSE WE ARE LAZY OR SOME OTHER REASON*. If a document contradicts the contract, then we need to discuss which one is better, *THERE SHOULDN'T BE ANY MORE OLD OR INCOHERENT DOCUMENTATION*
- When creating user stories, *MAKING SURE IT ACTUALLY CORRESPONDS WITH THE CONTRACT AND NOT SPLIT INTO 50 SUB USER STORIES THEN ADDING 5+ CLOSES WHEN OPENING A PR NOR SHOULD IT BE 80 USER STORIES MADE BY SOME AGENT AND INVENTED 85% OF THE ASPECTS OF OUR GAME*
- Ensuring the PRs are small and manageable and not 4000 lines of slop, unmaintainable and unreviewable code
- From day 1 there should be a good ci pipeline, reearch on how ci pipelines can work using gotdot and formatting code, NOT IN THE MIDDLE OF A PROJECT AFTER BASTIEN MENTIONED IT ONCE.

- for tests, RESEARCH ON THE BEST PRACTICES FOR TDD OR TESTING WHAT GOOD ASPECTS WE SHOULD TEST OUR GAME FIRSTT. There wont be any 500 feature or unit tests that no one knows what they are, nor THE TESTS SHOULD BE WRITTEN AFTER AN AGENT WROTE THE CODE, THAT'S POINTLESS.


### AI usage rules

1. Human writes the contract and ADRs, AI only critiques. Write `contract.md` by hand, then ask AI "what's contradictory or missing?" and keep only the points we understand.
    - ADRs written by an AI agent *MUST BE UNDERSTOOD AND AGREED ON*
2. Explain-it-back rule. Nobody merges or approves anything they can't explain in one sentence. When grilling, restate the question in our own words first and push back on at least one answer.
3. Cap AI output at the source. Prompt with a hard limit (e.g. max 30 lines, no filler). Anything AI-generated that goes into the repo gets a pair review before merge.
4. For every PR written by an agent, the description and the title *MUST BE WRITTEN BY A HUMAN TO ENSURE FULL UNDERSTANDING OF WHAT AN AGENT DID.*
5. Learning code stays human, AI is for boring or verifiable work. Write the Godot and C# fundamentals ourselves. Use AI for boilerplate, test scaffolding and benchmark scripts where we can check the output by running it.

### Documentation And contracts
- every piece of documentation should be in english, should be easy to read and understand and not 2000 lines of b.s
- there wont be any duplication of ADR'S or technical or domain decisions either, if one decision contractdicts another then that's the new contract and delete the old one
- the /docs folder should be small but still detailed especially for example the user stories, 
- user stories are also simple and vertically sliced. ISTG IF THERE ARE TASKS THAT ARE NOT VERICALLY SLICED IMMA CRASH OUT.
- if x or y person needs to discuss then dicuss, there's no point doing giant point for 50 minuted where we think we are productive when we are not.


