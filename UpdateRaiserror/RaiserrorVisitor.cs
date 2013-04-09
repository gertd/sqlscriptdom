using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace UpdateRaiserror
{
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
