using System.Collections.Generic;
using System.Data;

namespace RaspMat.Helpers
{
    internal class MathParser
    {

        private DataTable DataTab { get; } = new DataTable();

        public T Compute<T>(string equation, IDictionary<string, string> vars = null)
        {
            if (vars != null)
            {
                foreach (var variable in vars)
                {
                    equation = equation.Replace(variable.Key, variable.Value);
                }
            }

            return (T)DataTab.Compute(equation, null);
        }

        public T Compute<T>(string equation, IEnumerable<string> vars)
        {
            var dict = new Dictionary<string, string>();
            var count = 1;

            foreach (var variable in vars)
            {
                dict.Add("X" + count++.ToString(), variable);
            }

            return Compute<T>(equation.Replace('x', 'X'), dict);
        }

        public int ComputeInt(string equation) => Compute<int>(equation);
        public bool ComputeBool(string equation) => Compute<bool>(equation);

    }
}
