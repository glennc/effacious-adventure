namespace JsonConfigParser
{
    public static class TokenTypes
    {
        public const int EOF = 0;
        //Structural characters as defined in http://tools.ietf.org/html/rfc4627#section-2
        public const int BEGIN_ARRAY = 1; // [
        public const int END_ARRAY = 2; // ]
        public const int BEGIN_OBJECT = 3; // {
        public const int END_OBJECT = 4; // }
        public const int NAME_SEPARATOR = 5; // :
        public const int VALUE_SEPARATOR = 6; // ,

        //Literal token
        //TODO: Perhaps this should be three ints, one for true, false, and null?
        public const int LITERAL = 7; // true, false, null
        //VALUES 
        //This is probably not the right thing to do. The lexer should probably just stream back chars and the parser decides if it is a string or not.
        //But this does allow the lexer to handle whitespace appropriately..
        public const int STRING = 8;
    }
}