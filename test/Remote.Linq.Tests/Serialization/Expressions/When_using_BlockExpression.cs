﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Xunit;
    using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

    public abstract class When_using_BlockExpression
    {
#pragma warning disable SA1502 // Element should not be on a single line
#pragma warning disable SA1128 // Put constructor initializers on their own line

#if !NETCOREAPP1_0
        public class BinaryFormatter : When_using_BlockExpression
        {
            public BinaryFormatter() : base(BinarySerializationHelper.Serialize) { }
        }
#endif

#if NET
        public class NetDataContractSerializer : When_using_BlockExpression
        {
            public NetDataContractSerializer() : base(NetDataContractSerializationHelper.Serialize) { }
        }
#endif

        public class DataContractSerializer : When_using_BlockExpression
        {
            public DataContractSerializer() : base(DataContractSerializationHelper.SerializeExpression) { }
        }

        public class JsonSerializer : When_using_BlockExpression
        {
            public JsonSerializer() : base(JsonSerializationHelper.Serialize) { }
        }

        public class XmlSerializer : When_using_BlockExpression
        {
            public XmlSerializer() : base(XmlSerializationHelper.SerializeExpression) { }
        }

#pragma warning restore SA1128 // Put constructor initializers on their own line
#pragma warning restore SA1502 // Element should not be on a single line

        private Expression<Func<decimal, string>> _originalExpression;

        private RemoteLambdaExpression _remoteExpression;

        private RemoteLambdaExpression _serializedRemoteExpression;

        protected When_using_BlockExpression(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
        {
            ParameterExpression decimalInputParameter = Expression.Parameter(typeof(decimal));

            ParameterExpression formattedDecimalString = Expression.Variable(typeof(string));

            Expression<Func<decimal, string>> expression = Expression.Lambda<Func<decimal, string>>(
                Expression.Block(
                    new ParameterExpression[]
                    {
                        formattedDecimalString,
                    },
                    Expression.Assign(
                        decimalInputParameter,
                        Expression.Add(decimalInputParameter, Expression.Constant(1m))),
                    Expression.Assign(
                        formattedDecimalString,
                        Expression.Call(
                            decimalInputParameter,
                            typeof(decimal).GetMethod(nameof(decimal.ToString), Type.EmptyTypes))),
                    Expression.Call(
                        formattedDecimalString,
                        typeof(string).GetMethod(nameof(string.Replace), new[] { typeof(char), typeof(char) }),
                        Expression.Constant('.'),
                        Expression.Constant(','))),
                decimalInputParameter);

            _originalExpression = expression;

            _remoteExpression = expression.ToRemoteLinqExpression();

            _serializedRemoteExpression = serialize(_remoteExpression);
        }

        [Fact]
        public void Expression_block_result_should_be_equal()
        {
            var argument = 2.15m;

            string str1 = _originalExpression.Compile()(argument);

            string str2 = _remoteExpression.ToLinqExpression<decimal, string>().Compile()(argument);

            string str3 = _serializedRemoteExpression.ToLinqExpression<decimal, string>().Compile()(argument);

            "3,15"
                .ShouldMatch(str1)
                .ShouldMatch(str2)
                .ShouldMatch(str3);
        }
    }
}