//------------------------------------------------------------------------------
// <copyright file="RaiserrorVistor.cs" company="SQLProj.com">
//         Copyright © 2013 SQLProj.com - All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SqlProj.SqlScriptDom.Example.UpdateRaiserror
{
    using System.Collections.Generic;
    using Microsoft.SqlServer.TransactSql.ScriptDom;

    class RaiserrorVisitor : TSqlFragmentVisitor 
    {
        private List<RaiseErrorLegacyStatement> errNodes;

        public RaiserrorVisitor()
        {
            errNodes = new List<RaiseErrorLegacyStatement>();
        }

        public override void Visit(RaiseErrorLegacyStatement node)
        {
            base.Visit(node);

            errNodes.Add(node);
        }

        public List<RaiseErrorLegacyStatement> Nodes
        {
            get
            {
                return errNodes;
            }
        }
    }
}
