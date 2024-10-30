using GDWeave.Godot.Variants;
using GDWeave.Godot;
using GDWeave.Modding;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;

namespace webfishing_gun;

// I can't test that this works on multiplayer by myself...
// I can try attacking myself and hope that it will also work multiplayer?

// append to the start of the player punching code, the function that gets
// called when punched by others

public class DebugAttackSelfPatch : IScriptMod {
	public bool ShouldRun(string path) => path == "res://Scenes/Entities/Player/player.gdc";

	// returns a list of tokens for the new script, with the input being the original script's tokens
	public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens) {
		// wait for:
		//   func _punch(type = 0):

		var waiter = new MultiTokenWaiter([
            t => t.Type is TokenType.PrFunction, // 16047: Token(PrFunction, )
            t => t is IdentifierToken{ Name: "_punch" }, // 16048: IdentifierToken(Identifier, 741, _punch)
            t => t.Type is TokenType.ParenthesisOpen, // 16049: Token(ParenthesisOpen, )
            t => t.Type is TokenType.Identifier, // 16050: IdentifierToken(Identifier, 397, type)
            t => t.Type is TokenType.OpAssign, // 16051: Token(OpAssign, )
            t => t.Type is TokenType.Constant, // 16052: ConstantToken(Constant, 5, IntVariant(0))
            t => t.Type is TokenType.ParenthesisClose, // 16053: Token(ParenthesisClose, )
            t => t.Type is TokenType.Colon // 16054: Token(Colon, )
		]);

		// loop through all tokens in the script
		foreach (var token in tokens) {
			if (waiter.Check(token)) {
				// found our match, return the original token
				yield return token;

				// then add our own code:
				//   _punched(Vector3.ZERO, type)
				yield return new Token(TokenType.Newline, 1);
				yield return new IdentifierToken("_punched");
				yield return new Token(TokenType.ParenthesisOpen);
				yield return new Token(TokenType.BuiltInType, 7); // Vector3?
				yield return new Token(TokenType.Period);
				yield return new IdentifierToken("ZERO");
				yield return new Token(TokenType.Comma);
				yield return new IdentifierToken("type");
				yield return new Token(TokenType.ParenthesisClose);
			} else {
				// return the original token
				yield return token;
			}
		}
	}
}
