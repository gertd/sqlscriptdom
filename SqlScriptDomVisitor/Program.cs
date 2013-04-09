//------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SQLProj.com">
//         Copyright © 2013 SQLProj.com - All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SqlScriptDomVisitor
{
    using System;
    using System.Collections.Generic;
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

            bool initialQuotedIdentifiers = false;
            TSqlParser parser = new TSql110Parser(initialQuotedIdentifiers);

            StreamReader sr = input.OpenText();
            IList<ParseError> errors;

            TSqlFragment fragment = parser.Parse(sr, out errors);
            sr.Close();

            if (errors.Count > 0)
            {
                Console.WriteLine("Parse {0} errors input stream", errors.Count);
                return;
            }

            TableVisitor tableVisitor = new TableVisitor();
            fragment.Accept(tableVisitor);

            ViewVisitor viewVisitor = new ViewVisitor();
            fragment.Accept(viewVisitor);

            ProcVisitor procVisitor = new ProcVisitor();
            fragment.Accept(procVisitor);

            foreach (var table in tableVisitor.Nodes)
            {
                Console.WriteLine("table {0}.{1}",
                    table.SchemaObjectName.SchemaIdentifier.Value,
                    table.SchemaObjectName.BaseIdentifier.Value);
            }

            foreach(var view in viewVisitor.Nodes)
            {
                Console.WriteLine("view {0}.{1}",
                    view.SchemaObjectName.SchemaIdentifier.Value,
                    view.SchemaObjectName.BaseIdentifier.Value);
            }

            foreach(var proc in procVisitor.Nodes)
            {
                Console.WriteLine("proc {0}.{1}",
                    proc.ProcedureReference.Name.SchemaIdentifier.Value,
                    proc.ProcedureReference.Name.BaseIdentifier.Value);
            }
        }
    }
    
    class TableVisitor : TSqlFragmentVisitor
    {
        private List<CreateTableStatement> stmts;

        public TableVisitor()
        {
            stmts = new List<CreateTableStatement>();
        }

        public override void Visit(CreateTableStatement node)
        {
            base.Visit(node);
            stmts.Add(node);
        }

        public List<CreateTableStatement> Nodes
        {
            get
            {
                return stmts;
            }
        }
    }
    
    class ViewVisitor : TSqlFragmentVisitor
    {
        private List<CreateViewStatement> stmts;

        public ViewVisitor()
        {
            stmts = new List<CreateViewStatement>();
        }

        public override void Visit(CreateViewStatement node)
        {
            base.Visit(node);
            stmts.Add(node);
        }

        public List<CreateViewStatement> Nodes
        {
            get
            {
                return stmts;
            }
        }
    }
    
    class ProcVisitor : TSqlFragmentVisitor
    {
        private List<CreateProcedureStatement> stmts;

        public ProcVisitor()
        {
            stmts = new List<CreateProcedureStatement>();
        }

        public override void Visit(CreateProcedureStatement node)
        {
            base.Visit(node);
            stmts.Add(node);
        }

        public List<CreateProcedureStatement> Nodes
        {
            get
            {
                return stmts;
            }
        }
    }
}
