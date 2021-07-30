using System;
using System.Collections.Generic;

namespace Tools.Connections.WithoutAbstractFactory
{
    public class Command
    {
        internal string Query { get; private set; }
        internal bool IsStoredProcedure { get; private set; }
        internal IDictionary<string, object> Parameters { get; private set; }

        public Command(string query, bool isStoredProcedure)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("query is not valid !");

            Query = query;
            IsStoredProcedure = isStoredProcedure;
            Parameters = new Dictionary<string, object>();
        }

        public void AddParameter(string parameterName, object value)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
                throw new ArgumentException("parameterName is not valid !");

            Parameters.Add(parameterName, value ?? DBNull.Value);
        }
    }
}
