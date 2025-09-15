using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Financier.Desktop.Wizards;

using System.Threading; // For CancellationToken
// Remove any other using System.Linq.AsyncEnumerable or similar

namespace Financier.Desktop.Helpers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class MonobankHelper : IBankHelper
    {
        public async Task<IEnumerable<BankTransaction>> ParseReport(string filePath)
        {
            if (File.Exists(filePath))
            {
                await using FileStream file = File.OpenRead(filePath);
                using StreamReader streamReader = new StreamReader(file, Encoding.UTF8);
                using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    // Use a local alias to resolve ambiguity between AsyncEnumerable types
                    return await System.Linq.AsyncEnumerable.ToListAsync(
                        csv.GetRecordsAsync<BankTransaction>(),
                        CancellationToken.None
                    );
                }
            }
            return Array.Empty<BankTransaction>();
        }
    }
}
