using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace LibgenDesktop.Models.Entities
{
    internal class FictionBook : LibgenObject
    {
        private string authors;
        private string series;
        private string russianAuthor;

        public FictionBook()
            : base(LibgenObjectType.FICTION_BOOK)
        {
            authors = null;
            series = null;
            russianAuthor = null;
        }

        public string AuthorFamily1 { get; set; }
        public string AuthorName1 { get; set; }
        public string AuthorSurname1 { get; set; }
        public string Role1 { get; set; }
        public string Pseudonim1 { get; set; }
        public string AuthorFamily2 { get; set; }
        public string AuthorName2 { get; set; }
        public string AuthorSurname2 { get; set; }
        public string Role2 { get; set; }
        public string Pseudonim2 { get; set; }
        public string AuthorFamily3 { get; set; }
        public string AuthorName3 { get; set; }
        public string AuthorSurname3 { get; set; }
        public string Role3 { get; set; }
        public string Pseudonim3 { get; set; }
        public string AuthorFamily4 { get; set; }
        public string AuthorName4 { get; set; }
        public string AuthorSurname4 { get; set; }
        public string Role4 { get; set; }
        public string Pseudonim4 { get; set; }
        public string Series1 { get; set; }
        public string Series2 { get; set; }
        public string Series3 { get; set; }
        public string Series4 { get; set; }
        public string Title { get; set; }
        public string Format { get; set; }
        public string Version { get; set; }
        public long SizeInBytes { get; set; }
        public string Md5Hash { get; set; }
        public string Path { get; set; }
        public string Language { get; set; }
        public string Pages { get; set; }
        public string Identifier { get; set; }
        public string Year { get; set; }
        public string Publisher { get; set; }
        public string Edition { get; set; }
        public string Commentary { get; set; }
        public DateTime? AddedDateTime { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public string RussianAuthorFamily { get; set; }
        public string RussianAuthorName { get; set; }
        public string RussianAuthorSurname { get; set; }
        public string Cover { get; set; }
        public string GoogleBookId { get; set; }
        public string Asin { get; set; }
        public string AuthorHash { get; set; }
        public string TitleHash { get; set; }
        public string Visible { get; set; }

        public string Authors
        {
            get
            {
                if (authors == null)
                {
                    StringBuilder authorsStringBuilder = new StringBuilder();
                    AppendAuthorString(authorsStringBuilder, AuthorName1, AuthorSurname1, AuthorFamily1, Pseudonim1, Role1);
                    AppendAuthorString(authorsStringBuilder, AuthorName2, AuthorSurname2, AuthorFamily2, Pseudonim2, Role2);
                    AppendAuthorString(authorsStringBuilder, AuthorName3, AuthorSurname3, AuthorFamily3, Pseudonim3, Role3);
                    AppendAuthorString(authorsStringBuilder, AuthorName4, AuthorSurname4, AuthorFamily4, Pseudonim4, Role4);
                    authors = authorsStringBuilder.ToString();
                }
                return authors;
            }
        }

        public string Series
        {
            get
            {
                if (series == null)
                {
                    StringBuilder seriesStringBuilder = new StringBuilder();
                    foreach (string seriesItem in new[] { Series1, Series2, Series3, Series4 })
                    {
                        if (!String.IsNullOrWhiteSpace(seriesItem))
                        {
                            if (seriesStringBuilder.Length > 0)
                            {
                                seriesStringBuilder.Append("; ");
                            }
                            seriesStringBuilder.Append(seriesItem);
                        }
                    }
                    series = seriesStringBuilder.ToString();
                }
                return series;
            }
        }

        public string RussianAuthor
        {
            get
            {
                if (russianAuthor == null)
                {
                    StringBuilder russianAuthorStringBuilder = new StringBuilder();
                    AppendAuthorString(russianAuthorStringBuilder, RussianAuthorName, RussianAuthorSurname, RussianAuthorFamily, null, null);
                    russianAuthor = russianAuthorStringBuilder.ToString();
                }
                return russianAuthor;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendAuthorString(StringBuilder stringBuilder, string name, string surname, string familyName, string pseudonim, string role)
        {
            bool hasName = !String.IsNullOrWhiteSpace(name);
            bool hasSurname = !String.IsNullOrWhiteSpace(surname);
            bool hasFamilyName = !String.IsNullOrWhiteSpace(familyName);
            bool hasPseudonim = !String.IsNullOrWhiteSpace(pseudonim);
            bool hasRole = !String.IsNullOrWhiteSpace(role);
            if (hasName || hasSurname || hasFamilyName || hasPseudonim || hasRole)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append("; ");
                }
                bool firstPart = true;
                if (hasName)
                {
                    stringBuilder.Append(name);
                    firstPart = false;
                }
                if (hasSurname)
                {
                    if (!firstPart)
                    {
                        stringBuilder.Append(" ");
                    }
                    stringBuilder.Append(surname);
                    firstPart = false;
                }
                if (hasFamilyName)
                {
                    if (!firstPart)
                    {
                        stringBuilder.Append(" ");
                    }
                    stringBuilder.Append(familyName);
                    firstPart = false;
                }
                if (hasPseudonim)
                {
                    if (!firstPart)
                    {
                        stringBuilder.Append(" (");
                    }
                    stringBuilder.Append(pseudonim);
                    if (!firstPart)
                    {
                        stringBuilder.Append(")");
                    }
                    firstPart = false;
                }
                if (hasRole)
                {
                    if (!firstPart)
                    {
                        stringBuilder.Append(" ");
                    }
                    stringBuilder.Append("(");
                    stringBuilder.Append(role);
                    stringBuilder.Append(")");
                }
            }
        }
    }
}
