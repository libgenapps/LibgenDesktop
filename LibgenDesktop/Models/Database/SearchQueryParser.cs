using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Database
{
    internal class SearchQueryParser
    {
        private readonly string originalSearchQuery;

        private SearchQueryParser(string originalSearchQuery)
        {
            this.originalSearchQuery = originalSearchQuery;
        }

        public static string GetEscapedQuery(string originalSearchQuery)
        {
            return new SearchQueryParser(originalSearchQuery).GetEscapedQuery();
        }

        private static void AddSearchQueryPart(List<string> searchQueryBuilder, string searchQueryPart)
        {
            if (searchQueryPart.StartsWith("\""))
            {
                searchQueryBuilder.Add(searchQueryPart);
            }
            else
            {
                switch (searchQueryPart)
                {
                    case "AND":
                    case "OR":
                    case "NOT":
                        searchQueryBuilder.Add(searchQueryPart);
                        break;
                    default:
                        if (searchQueryPart.Length > 1 && searchQueryPart.EndsWith("*"))
                        {
                            searchQueryBuilder.Add($"\"{searchQueryPart.Substring(0, searchQueryPart.Length - 1)}\"*");
                        }
                        else
                        {
                            searchQueryBuilder.Add($"\"{searchQueryPart}\"");
                        }
                        break;
                }
            }
        }

        private string GetEscapedQuery()
        {
            List<string> searchQueryBuilder = new List<string>();
            bool isInQuotes = false;
            int currentIndex = 0;
            string currentQueryPart = String.Empty;
            while (currentIndex < originalSearchQuery.Length)
            {
                char currentChar = originalSearchQuery[currentIndex];
                switch (currentChar)
                {
                    case '"':
                        if (isInQuotes)
                        {
                            currentQueryPart += currentChar;
                            isInQuotes = false;
                            if ((currentIndex < originalSearchQuery.Length - 1) && (originalSearchQuery[currentIndex + 1] == '*'))
                            {
                                currentIndex++;
                                currentQueryPart += '*';
                            }
                            if (currentQueryPart.Length > 0)
                            {
                                AddSearchQueryPart(searchQueryBuilder, currentQueryPart);
                                currentQueryPart = String.Empty;
                            }
                        }
                        else
                        {
                            if (currentQueryPart.Length > 0)
                            {
                                AddSearchQueryPart(searchQueryBuilder, currentQueryPart);
                            }
                            currentQueryPart = currentChar.ToString();
                            isInQuotes = true;
                        }
                        break;
                    case ' ':
                        if (isInQuotes)
                        {
                            currentQueryPart += currentChar;
                        }
                        else
                        {
                            if (currentQueryPart.Length > 0)
                            {
                                AddSearchQueryPart(searchQueryBuilder, currentQueryPart);
                                currentQueryPart = String.Empty;
                            }
                        }
                        break;
                    default:
                        currentQueryPart += currentChar;
                        break;
                }
                currentIndex++;
            }
            if (currentQueryPart.Length > 0)
            {
                if (isInQuotes)
                {
                    currentQueryPart = currentQueryPart.Substring(1);
                }
                foreach (string remainingQueryPart in currentQueryPart.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    AddSearchQueryPart(searchQueryBuilder, remainingQueryPart);
                }
            }
            return String.Join(" ", searchQueryBuilder);
        }
    }
}
