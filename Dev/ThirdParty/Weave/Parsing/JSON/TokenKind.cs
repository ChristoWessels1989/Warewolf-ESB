﻿using System.Collections.Generic;
using System.Parsing.Tokenization;

namespace System.Parsing.JSON
{
    public sealed class TokenKind : TokenDefinition
    {
        #region Readonly Fields
        public static readonly List<TokenKind> Operators = new List<TokenKind>();
        public static readonly List<TokenKind> TypeLiterals = new List<TokenKind>();

        public static readonly TokenKind Unknown = new TokenKind("Unknown", "", TokenClassification.Unknown);
        public static readonly TokenKind Whitespace = new TokenKind("Whitespace", " ", TokenClassification.Whitespace);
        public static readonly TokenKind LineBreak = new TokenKind("Line Break", StringUtility.StrNewLine, TokenClassification.Whitespace);
        public static readonly TokenKind EOF = new TokenKind("End Of File", StringUtility.StrNewLine, TokenClassification.EndOfFile);

        public static readonly TokenKind Comma = CreateOperator(",", "Comma");

        public static readonly TokenKind Colon = CreateOperator(":", "Colon");

        public static readonly TokenKind LeftCurlyBracket = CreateOperator("{", "Left Curly Bracket");
        public static readonly TokenKind RightCurlyBracket = CreateOperator("}", "Right Curly Bracket");

        public static readonly TokenKind RegularChar = CreateCharLiteral("", "Regular Character");
        public static readonly TokenKind HexadecimalChar = CreateCharLiteral("", "Hexadecimal Character");
        public static readonly TokenKind UnicodeChar = CreateCharLiteral("", "Unicode Character");
        public static readonly TokenKind EscapedChar = CreateCharLiteral("", "Escaped Character");

        public static readonly TokenKind RegularString = CreateStringLiteral("", "Regular String");
        public static readonly TokenKind VerbatimString = CreateStringLiteral("", "Verbatim String");

        public static readonly TokenKind IntegerNoSuffix = CreateIntegerLiteral("", "Integer No Suffix");
        public static readonly TokenKind IntegerHexadecimal = CreateIntegerLiteral("", "Integer Hexadecimal");
        public static readonly TokenKind IntegerSuffixU = CreateIntegerLiteral("", "Integer Unsigned");
        public static readonly TokenKind IntegerSuffixL = CreateIntegerLiteral("", "Integer Long");
        public static readonly TokenKind IntegerSuffixUL = CreateIntegerLiteral("", "Integer ULong");

        public static readonly TokenKind RealNoSuffix = CreateRealLiteral("", "Real No Suffix");
        public static readonly TokenKind RealSuffixF = CreateRealLiteral("", "Real Float");
        public static readonly TokenKind RealSuffixD = CreateRealLiteral("", "Real Double");
        public static readonly TokenKind RealSuffixPercent = CreateRealLiteral("", "Real Double Percent");

        public static readonly TokenKind BooleanFalse = CreateBooleanLiteral("false", "Boolean False");
        public static readonly TokenKind BooleanTrue = CreateBooleanLiteral("true", "Boolean True");

        public static readonly HashSet<TokenKind> OperandTerminators = CreateOperandTerminators();
        public static readonly HashSet<TokenKind> BinaryOperators = CreateBinaryOperators();
        public static readonly HashSet<TokenKind> RelationalOperators = CreateRelationalOperators();
        public static readonly TokenKind[] Pairs = CreateTokenPairs();
        #endregion

        #region Static Members
        private static HashSet<TokenKind> CreateOperandTerminators()
        {
            HashSet<TokenKind> set = new HashSet<TokenKind>();


            return set;
        }

        private static HashSet<TokenKind> CreateBinaryOperators()
        {
            HashSet<TokenKind> set = new HashSet<TokenKind>();

            return set;
        }

        private static HashSet<TokenKind> CreateRelationalOperators()
        {
            HashSet<TokenKind> set = new HashSet<TokenKind>();

            return set;
        }

        private static TokenKind[] CreateTokenPairs()
        {
            TokenKind[] pairs = new TokenKind[GetTotalDefinitionsOfType(typeof(TokenKind))];

            pairs[LeftCurlyBracket._serial] = RightCurlyBracket;

            pairs[RightCurlyBracket._serial] = LeftCurlyBracket;

            return pairs;
        }

        private static TokenKind CreateOperator(string identifier, string name)
        {
            return CreateOperator(identifier, name, true);
        }

        private static TokenKind CreateOperator(string identifier, string name, bool add)
        {
            TokenKind toReturn = new TokenKind(name, identifier, TokenClassification.Operator);
            if (add) Operators.Add(toReturn);
            return toReturn;
        }

        private static TokenKind CreateCharLiteral(string identifier, string name)
        {
            TokenKind toReturn = new TokenKind(name, identifier, TokenClassification.CharLiteral);
            return toReturn;
        }

        private static TokenKind CreateStringLiteral(string identifier, string name)
        {
            TokenKind toReturn = new TokenKind(name, identifier, TokenClassification.StringLiteral);
            return toReturn;
        }

        private static TokenKind CreateIntegerLiteral(string identifier, string name)
        {
            TokenKind toReturn = new TokenKind(name, identifier, TokenClassification.IntegerLiteral);
            return toReturn;
        }

        private static TokenKind CreateRealLiteral(string identifier, string name)
        {
            TokenKind toReturn = new TokenKind(name, identifier, TokenClassification.RealLiteral);
            return toReturn;
        }

        private static TokenKind CreateBooleanLiteral(string identifier, string name)
        {
            TokenKind toReturn = new TokenKind(name, identifier, TokenClassification.BooleanLiteral);
            return toReturn;
        }
        #endregion

        #region Instance Fields
        private TokenClassification _kind;
        private bool _isKeyword;
        #endregion

        #region Public Properties
        public bool IsInvalid { get { return _kind == TokenClassification.Invalid; } }
        public override bool IsUnknown { get { return _kind == TokenClassification.Unknown; } }
        public override bool IsWhitespace { get { return _kind == TokenClassification.Whitespace; } }
        public override bool IsEndOfFile { get { return _kind == TokenClassification.EndOfFile; } }
        public override bool IsKeyword { get { return _isKeyword; } }
        public bool IsOperator { get { return _kind == TokenClassification.Operator; } }
        public bool IsStringLiteral { get { return _kind == TokenClassification.StringLiteral; } }
        public bool IsCharLiteral { get { return _kind == TokenClassification.CharLiteral; } }
        public bool IsIntegerLiteral { get { return _kind == TokenClassification.IntegerLiteral; } }
        public bool IsRealLiteral { get { return _kind == TokenClassification.RealLiteral; } }
        public bool IsBooleanLiteral { get { return _kind == TokenClassification.BooleanLiteral; } }
        public bool IsNullLiteral { get { return _kind == TokenClassification.NullLiteral; } }
        public bool IsTypeLiteral { get { return _kind == TokenClassification.TypeLiteral; } }
        public TokenClassification Kind { get { return _kind; } }
        #endregion

        private TokenKind(string name, string identifier, TokenClassification classification)
            : base(name, identifier)
        {
            _kind = classification;
            if (_kind == TokenClassification.Unknown) _isKeyword = true;
        }

        private TokenKind(string name, string identifier, TokenClassification classification, bool isKeyword)
            : base(name, identifier)
        {
            _kind = classification;
            _isKeyword = isKeyword;
        }
    }

    public enum TokenClassification
    {
        Invalid = -1,
        Unknown = 0,
        Whitespace = 1,
        EndOfFile = 2,
        Operator = 3,
        StringLiteral = 4,
        CharLiteral = 5,
        IntegerLiteral = 6,
        RealLiteral = 7,
        BooleanLiteral = 8,
        NullLiteral = 9,
        TypeLiteral = 10,
    }
}
