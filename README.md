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
- or can jump over footballers.
    - jump rules: the user can jump over one or ore footballers in one direction (vertically, horizontally or diagonally), only, if there is an empty field after them, or if the ball would reach the goal line. If the ball would go out of field other than the goal line, the jump cannot be done.
    - if the player did a jump, and the ball can continue with an other jump, the player as to do it until there are no more jumps that can be done
    - if the ball can jump in multiple directions, the player has to decide
- all the footballers that were jumped over, have to be removed from the field and put back into the stack, from where they can be placed again.
- the game is over if the ball goes over one of the two goal lines - when it reaches the first or the last column (the goalpost of Player1 and 2, respectively). 

### Game engine
The game acts as a black box. It receives "Commands" (e.g. Player1 puts a footballer on coordinate X-Y) and returns with "Events" that have happened during this step (e.g. FootballerPlaced(X,Y)). The game engine validates the incoming command and returns an error if it is not valid (e.g. Player1 wants to do something, although it's Player2's turn).

#### Commands
Everything that can direct the flow of the game has to arrive as a Command. It is going to be validated first, then the inner state of the game is changed and Events are produced, that are going to be returned. Later the Commands are going to come from the UI, and the Events will be used to update the UI to the user.
The commands are symettrical to the users. Events too.
Possible Commands:
- StartGame(GameConfig)
- PlaceFootballer(Player, Coordinate)
- MoveBall(Player, Coordinate)

#### Events
Events signal the changes in the inner state. (TODO: elaborate)
Possible Events:
- FootballerPlaced(Coordinate)
- BallPlaced(Coordinate)
- FootballerRemoved(Coordinate)
- GameWonBy(Player)
- CurrentPlayerChanged(Player)