using GDWeave.Godot.Variants;
using GDWeave.Godot;
using GDWeave.Modding;

namespace webfishing_gun;

// append to player getting punched code. if the other player doesn't have
// the mod (is that possible?) then I guess they will get punched normally,
// default gloves punch, as the punch type of 1 is the only punch type that
// changes anything - but I don't own these gloves to test that (i'm poor).
// otherwise this will do a default punch + drown animation + return to spawn.
// that's as close as I could get to killing the player :P

// also writing code in tokens sucks ass. ideally I would be able to find a
// GDScript string, and insert a GDScript string, and have it compiled during
// runtime to convert these strings into tokens.
// I don't imagine that would be easy to implement into the loader though...
// regardless, the token system - whilst painful - functions, which is awesome!

public class RespawnOnAttackedWithGunPatch : IScriptMod {
	public bool ShouldRun(string path) => path == "res://Scenes/Entities/Player/player.gdc";

	// returns a list of tokens for the new script, with the input being the original script's tokens
	public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens) {
		// wait for:
		// 	  match type:
		//      0:
		//        bounce_horz = 4.0
		//        bounce_vert = 8.0
		//      1:
		//        bounce_horz = 12.0
		//        bounce_vert = 24.0
		var waiter = new MultiTokenWaiter([
            t => t.Type is TokenType.CfMatch, //16253: Token(CfMatch, )
            t => t.Type is TokenType.Identifier, //16254: IdentifierToken(Identifier, 397, type)
            t => t.Type is TokenType.Colon, //16255: Token(Colon, )
            t => t.Type is TokenType.Newline, //16256: Token(Newline, 2)
            t => t.Type is TokenType.Constant, //16257: ConstantToken(Constant, 5, IntVariant(0))
            t => t.Type is TokenType.Colon, //16258: Token(Colon, )
            t => t.Type is TokenType.Newline, //16259: Token(Newline, 3)
            t => t.Type is TokenType.Identifier, //16260: IdentifierToken(Identifier, 697, bounce_horz)
            t => t.Type is TokenType.OpAssign, //16261: Token(OpAssign, )
            t => t.Type is TokenType.Constant, //16262: ConstantToken(Constant, 316, RealVariant(4))
            t => t.Type is TokenType.Newline, //16263: Token(Newline, 3)
            t => t.Type is TokenType.Identifier, //16264: IdentifierToken(Identifier, 698, bounce_vert)
            t => t.Type is TokenType.OpAssign, //16265: Token(OpAssign, )
            t => t.Type is TokenType.Constant, //16266: ConstantToken(Constant, 530, RealVariant(8))
            t => t.Type is TokenType.Newline, //16267: Token(Newline, 2)
            t => t.Type is TokenType.Constant, //16268: ConstantToken(Constant, 9, IntVariant(1))
            t => t.Type is TokenType.Colon, //16269: Token(Colon, )
            t => t.Type is TokenType.Newline, //16270: Token(Newline, 3)
            t => t.Type is TokenType.Identifier, //16271: IdentifierToken(Identifier, 697, bounce_horz)
            t => t.Type is TokenType.OpAssign, //16272: Token(OpAssign, )
            t => t.Type is TokenType.Constant, //16273: ConstantToken(Constant, 185, RealVariant(12))
            t => t.Type is TokenType.Newline, //16274: Token(Newline, 3)
            t => t.Type is TokenType.Identifier, //16275: IdentifierToken(Identifier, 698, bounce_vert)
            t => t.Type is TokenType.OpAssign, //16276: Token(OpAssign, )
            t => t.Type is TokenType.Constant, //16277: ConstantToken(Constant, 186, RealVariant(24))
		]);

		// loop through all tokens in the script
		foreach (var token in tokens) {
			if (waiter.Check(token)) {
				// found our match, return the original token
				yield return token;

				// then add our own code:
				//   6780270: # "gun" in hex ascii!
                //     _return_to_spawn()
                //     _enter_animation("drown", true, true)
				yield return new Token(TokenType.Newline, 2);
				yield return new ConstantToken(new IntVariant(6780270));
				yield return new Token(TokenType.Colon);
				yield return new Token(TokenType.Newline, 3);
				yield return new IdentifierToken("_return_to_spawn");
				yield return new Token(TokenType.ParenthesisOpen);
				yield return new Token(TokenType.ParenthesisClose);
				yield return new Token(TokenType.Newline, 3);
				yield return new IdentifierToken("_enter_animation");
				yield return new Token(TokenType.ParenthesisOpen);
				yield return new ConstantToken(new StringVariant("drown"));
				yield return new Token(TokenType.Comma);
				yield return new ConstantToken(new BoolVariant(true));
				yield return new Token(TokenType.Comma);
				yield return new ConstantToken(new BoolVariant(true));
				yield return new Token(TokenType.ParenthesisClose);
			} else {
				// return the original token
				yield return token;
			}
		}
	}
}
