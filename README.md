## How it's made

### Initial project structure
1. create a blank solution
2. create a class library project
3. create a xunit test project
5. add reference to the library project from the test project

### Game start
Game has to be started with specific settings. We should provide two player names, the size of the board (number of rows, number of columns - both should be odd so that the ball can be placed exactly in the middle, and columns count should be at least 3, because if it's one, the ball is already behind the goal line), and the number of footballers - it should be even, so that they can be evenly divided between the two players. It makes sense to encapsulate these settings in a GameConfig, which is going to be responsible to validate these values - it won't allow receiving invalid settings.

### Game rules
The game will start by the first player, who can 
- either place a footballer anywhere on the field where there is free space (no other footballers, no ball)
- or can jump over one or more footballers in one direction. 