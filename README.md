# aggravationSL

Aggravation Board Game
My family members have been playing this board game for some time now and I wondered if I could create a computer version of it.  So I chose Silverlight and gave it a shot.

I used the Prism modularity patterns to make this a modular application.  This allows me to easily swap out the game board designs with different ones just by changing the module catalog.  I supply two different game boards.  Simply specify one to use in Catalog.xaml. 

The gameboards are set up using Silverlight Behaviors to specify the elements that make up the game board.  This allows game boards to be quickly created in Expression Blend.

This was built on Silverlight 4, but I am looking to update it to Silverlight 5 soon.  There are few known issues in this first release, and is pretty stable right now.

You can try it out at this link to my server:
http://mcpro.com/aggravation/

Please mail me with any questions or interest on this project.
dotnethero@mcpro.com









Object
The first player to move all four player pieces from the base area to the home area wins. On a player's turn, the player draws a card from the deck. The player moves along the game track the number of spaces equal to the card value. The cards 2-10 are face value, and the Ace, King, Queen, Jack, and Joker cards have a value of 1.

Gameplay
The player moves from base to start by drawing a Joker, Ace, or Six. When one of these cards is drawn, the player moves to the start location and no further. Only one piece can occupy the start location. The player draws again. Anytime the player draws a Six, Ace, King, Queen, Jack, or Joker they get to take another turn. The player moves around the game board clockwise the full card value until they reach their own home area. You must move the full card value, and must move into home the exact number of moves. 

Regular moves
Players cannot land on or move past their own piece, but can move past other players. 
If a player lands on another player, then the landed on player is sent back to their base. If the player draws a four, then the player must move backwards four spaces.

Fast Track and Super Fast Track
The inner locations in the triangles are fast track locations. If a player lands on one of these locations by exact count, they are on the fast track. The player can move clockwise between fast track locations on their next turn. This allows a player to advance the game board quickly. If the fast track move is not used in the next turn, then the player loses the chance to move along the fast track. You can move into the regular game track for the remaining card value at any fast track location.

The super fast track is the location in the middle of the game board. You can move to the super fast track location by exact count. The only way out of super fast track, is to draw a card value of one. You can exit into any of the fast track locations.

Last edited Sep 29, 2012 at 4:53 AM by dotnethero, version 18
