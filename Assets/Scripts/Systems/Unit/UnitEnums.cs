public enum Team { Player, Enemy }

/*
Idle: Waiting around to be told what to do
Tapped: Its turn is spent and it can't be told to take an action
Moving: Actively moving on the screen from point a to point b
Selected: When selected, its movement/attack range shows up and you are selecting where its gonna go
Action: Usually the state that comes after Selected, but is the state where the actionmenu is up and the player is choosing what the unit to do
Might add more later but I think this is good for now.
*/
public enum UnitState { Idle, Tapped, Moving, Selected, Action, Cutscene }