//------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SQLProj.com">
//         Copyright © 2013 SQLProj.com - All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SqlProj.SqlScriptDom.Example.UpdateRaiserror
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.SqlServer.TransactSql.ScriptDom;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() < 1)
            {
                Console.WriteLine("No input file specified");
                return;
            }

            FileInfo input = new FileInfo(args[0]); 

            SqlScriptGeneratorOptions scriptOptions = new SqlScriptGeneratorOptions()
            {
                AlignClauseBodies = true,
                AlignColumnDefinitionFields = true,
                AlignSetClauseItem = true,
                AsKeywordOnOwnLine = true,
                IncludeSemicolons = true,
                IndentationSize = 4,
                IndentSetClause = true,
                IndentViewBody = true,
                KeywordCasing = KeywordCasing.Lowercase,
                MultilineInsertSourcesList = true,
                MultilineInsertTargetsList = true,
                MultilineSelectElementsList = true,
                MultilineSetClauseItems = true,
                MultilineViewColumnsList = true,
                MultilineWherePredicatesList = true,
                NewLineBeforeCloseParenthesisInMultilineList = true,
                NewLineBeforeFromClause = true,
                NewLineBeforeGroupByClause = true,
                NewLineBeforeHavingClause = true,
                NewLineBeforeJoinClause = true,
                NewLineBeforeOffsetClause = true,
                NewLineBeforeOpenParenthesisInMultilineList = true,
                NewLineBeforeOrderByClause = true,
                NewLineBeforeOutputClause = true,
                NewLineBeforeWhereClause = true,
                SqlVersion = SqlVersion.Sql110
            };

            bool initialQuotedIdentifiers = false;
            TSqlParser parser = new TSql80Parser(initialQuotedIdentifiers);
            Sql80ScriptGenerator scriptGen = new Sql80ScriptGenerator(scriptOptions);

            StreamReader sr = input.OpenText();
            IList<ParseError> errors;

            TSqlFragment fragment = parser.Parse(sr, out errors);
            sr.Close();

            if (errors.Count > 0)
            {
                Console.WriteLine("Parse {0} errors input stream", errors.Count);
                return;
            }

            RaiserrorVisitor visitor = new RaiserrorVisitor();
            fragment.Accept(visitor);

            var newTokenStream = new List<TSqlParserToken>();
            
            for (int n = 0; n < visitor.Nodes.Count; n++)
            {
                var node = visitor.Nodes[n];

                var beginTokenIndex = (n == 0 ? 0 : node.FirstTokenIndex);
                var firstTokenIndex = node.FirstTokenIndex;
                var lastTokenIndex = node.LastTokenIndex;
                var nextTokenIndex = (n < (visitor.Nodes.Count - 1) ? visitor.Nodes[n + 1].FirstTokenIndex : node.ScriptTokenStream.Count);

                var msg = node.SecondParameter as StringLiteral;

                int severity = 10;
                int state = 0;
                string errmsg = msg.Value;

                RaiseErrorStatement newErr = new RaiseErrorStatement()
                {
                    FirstParameter = CreateStringLiteral(errmsg),
                    SecondParameter = CreateIntegerLiteral(severity),
                    ThirdParameter = CreateIntegerLiteral(state)
                };

                var errTokens = scriptGen.GenerateTokens(newErr);

                // copy token till element where replaced statement begins
                for (int i = beginTokenIndex; i < firstTokenIndex; i++)
                {
                    Console.WriteLine("Token: [{0}] Text: [{1}]", node.ScriptTokenStream[i].TokenType.ToString(), node.ScriptTokenStream[i].Text);
                    newTokenStream.Add(node.ScriptTokenStream[i]);
                }

                // inject new statement
                for (int i = 0; i < errTokens.Count; i++)
                {
                    Console.WriteLine("Token: [{0}] Text: [{1}]", errTokens[i].TokenType.ToString(), errTokens[i].Text);
                    newTokenStream.Add(errTokens[i]);
                }

                // copy tokens between last and next, if last token, till end of stream
                for (int i = lastTokenIndex + 1; i < nextTokenIndex; i++)
                {
                    Console.WriteLine("Token: [{0}] Text: [{1}]", node.ScriptTokenStream[i].TokenType.ToString(), node.ScriptTokenStream[i].Text);
                    newTokenStream.Add(node.ScriptTokenStream[i]);
                }
            }

            TSqlFragment newFragment = parser.Parse(newTokenStream, out errors);
            if (errors.Count > 0)
            {
                Console.WriteLine("Parse {0} errors output stream", errors.Count);
                return;
            }

            StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
            scriptGen.GenerateScript(newFragment, sw);
            
            Console.WriteLine("-------------------------------------------------------");
            for (int i = 0; i < newTokenStream.Count; i++)
            {
                Console.Write(newTokenStream[i].Text);
            }
            Console.WriteLine("-------------------------------------------------------");
            
        }

        public static StringLiteral CreateStringLiteral(string value, bool isNational = true)
        {
            StringLiteral ast = null;

            if (value != null)
            {
                ast = new StringLiteral()
                {
                    Value = value,
                    IsNational = isNational
                };
            }

            return ast;
        }

        public static IntegerLiteral CreateIntegerLiteral(int value)
        {
            return CreateIntegerLiteral(value.ToString(CultureInfo.InvariantCulture));
        }

        public static IntegerLiteral CreateIntegerLiteral(string value)
        {
            IntegerLiteral ast = null;

            if (value != null)
            {
                ast = new IntegerLiteral()
                {
                    Value = value
                };
            }

            return ast;
        }
    }
}
