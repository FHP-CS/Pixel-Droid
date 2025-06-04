// RuntimeError class remains the same
using System;
using PixelWallE.Common;
    public class RuntimeError : Exception
    {
        public Token Token { get; }
        public RuntimeError(string message, Token token) : base(message)
        {
            Token = token;
        }
    }