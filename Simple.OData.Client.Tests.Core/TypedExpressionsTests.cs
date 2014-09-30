﻿using System;
using System.Linq.Expressions;
using Xunit;

namespace Simple.OData.Client.Tests
{
    public class TypedExpressionTests : TestBase
    {
        class TestEntity
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public Guid LinkID { get; set; }
            public decimal Price { get; set; }
            public DateTime CreationTime { get; set; }
            public DateTimeOffset Updated { get; set; }
            public TimeSpan Period { get; set; }
        }

        [Fact]
        public void And()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID == 1 && x.ProductName == "Chai";
            Assert.Equal("ProductID eq 1 and ProductName eq 'Chai'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void Or()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName == "Chai" || x.ProductID == 1;
            Assert.Equal("ProductName eq 'Chai' or ProductID eq 1", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void Not()
        {
            Expression<Func<TestEntity, bool>> filter = x => !(x.ProductName == "Chai");
            Assert.Equal("not(ProductName eq 'Chai')", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void Precedence()
        {
            Expression<Func<TestEntity, bool>> filter = x => (x.ProductID == 1 || x.ProductID == 2) && x.ProductName == "Chai";
            Assert.Equal("(ProductID eq 1 or ProductID eq 2) and ProductName eq 'Chai'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void EqualString()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName == "Chai";
            Assert.Equal("ProductName eq 'Chai'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void EqualNumeric()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID == 1;
            Assert.Equal("ProductID eq 1", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void NotEqualNumeric()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID != 1;
            Assert.Equal("ProductID ne 1", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void GreaterNumeric()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID > 1;
            Assert.Equal("ProductID gt 1", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void GreaterOrEqualNumeric()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID >= 1.5;
            Assert.Equal("ProductID ge 1.5", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void LessNumeric()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID < 1;
            Assert.Equal("ProductID lt 1", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void LessOrEqualNumeric()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID <= 1;
            Assert.Equal("ProductID le 1", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void AddEqualNumeric()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID + 1 == 2;
            Assert.Equal("ProductID add 1 eq 2", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void SubEqualNumeric()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID - 1 == 2;
            Assert.Equal("ProductID sub 1 eq 2", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void MulEqualNumeric()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID * 1 == 2;
            Assert.Equal("ProductID mul 1 eq 2", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void DivEqualNumeric()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID / 1 == 2;
            Assert.Equal("ProductID div 1 eq 2", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void ModEqualNumeric()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID % 1 == 2;
            Assert.Equal("ProductID mod 1 eq 2", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void EqualLong()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductID == 1L;
            Assert.Equal("ProductID eq 1L", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void EqualDecimal()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.Price == 1M;
            Assert.Equal("Price eq 1.00M", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void EqualDecimalWithFractionalPart()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.Price == 1.23M;
            Assert.Equal("Price eq 1.23M", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void EqualGuid()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.LinkID == Guid.Empty;
            Assert.Equal("LinkID eq guid'00000000-0000-0000-0000-000000000000'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void EqualDateTime()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.CreationTime == new DateTime(2013, 1, 1);
            Assert.Equal("CreationTime eq datetime'2013-01-01T00:00:00.0000000Z'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void EqualDateTimeOffset()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.Updated == new DateTimeOffset(new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            Assert.Equal("Updated eq datetimeoffset'2013-01-01T00:00:00.0000000+00:00'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void EqualTimeSpan()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.Period == new TimeSpan(1, 2, 3);
            Assert.Equal("Period eq time'01:02:03'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void LengthOfStringEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.Length == 4;
            Assert.Equal("length(ProductName) eq 4", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void StringToLowerEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.ToLower() == "chai";
            Assert.Equal("tolower(ProductName) eq 'chai'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void StringToUpperEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.ToUpper() == "CHAI";
            Assert.Equal("toupper(ProductName) eq 'CHAI'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void StringStartsWithEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.StartsWith("Ch") == true;
            Assert.Equal("startswith(ProductName,'Ch') eq true", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void StringEndsWithEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.EndsWith("Ch") == true;
            Assert.Equal("endswith(ProductName,'Ch') eq true", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void StringContainsEqualTrue()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.Contains("ai") == true;
            Assert.Equal("substringof('ai',ProductName) eq true", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void StringContainsEqualFalse()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.Contains("ai") == false;
            Assert.Equal("substringof('ai',ProductName) eq false", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void StringContains()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.Contains("ai");
            Assert.Equal("substringof('ai',ProductName)", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void StringNotContains()
        {
            Expression<Func<TestEntity, bool>> filter = x => !x.ProductName.Contains("ai");
            Assert.Equal("not substringof('ai',ProductName)", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void IndexOfStringEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.IndexOf("ai") == 1;
            Assert.Equal("indexof(ProductName,'ai') eq 1", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void SubstringWithPositionEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.Substring(1) == "hai";
            Assert.Equal("substring(ProductName,1) eq 'hai'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void SubstringWithPositionAndLengthEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.Substring(1, 2) == "ha";
            Assert.Equal("substring(ProductName,1,2) eq 'ha'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void ReplaceStringEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.Replace("a", "o") == "Choi";
            Assert.Equal("replace(ProductName,'a','o') eq 'Choi'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void TrimEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.ProductName.Trim() == "Chai";
            Assert.Equal("trim(ProductName) eq 'Chai'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void ConcatEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => string.Concat(x.ProductName, "Chai") == "ChaiChai";
            Assert.Equal("concat(ProductName,'Chai') eq 'ChaiChai'", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void DayEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.CreationTime.Day == 1;
            Assert.Equal("day(CreationTime) eq 1", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void MonthEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.CreationTime.Month == 2;
            Assert.Equal("month(CreationTime) eq 2", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void YearEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.CreationTime.Year == 3;
            Assert.Equal("year(CreationTime) eq 3", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void HourEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.CreationTime.Hour == 4;
            Assert.Equal("hour(CreationTime) eq 4", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void MinuteEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.CreationTime.Minute == 5;
            Assert.Equal("minute(CreationTime) eq 5", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void SecondEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => x.CreationTime.Second == 6;
            Assert.Equal("second(CreationTime) eq 6", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

#if !NETFX_CORE
        [Fact]
        public void RoundEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => decimal.Round(x.Price) == 1;
            Assert.Equal("round(Price) eq 1.00M", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }
#endif

        [Fact]
        public void FloorEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => decimal.Floor(x.Price) == 1;
            Assert.Equal("floor(Price) eq 1.00M", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }

        [Fact]
        public void CeilingEqual()
        {
            Expression<Func<TestEntity, bool>> filter = x => decimal.Ceiling(x.Price) == 2;
            Assert.Equal("ceiling(Price) eq 2.00M", ODataExpression.FromLinqExpression(filter).AsString(_session));
        }
    }
}
