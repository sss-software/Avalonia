﻿using System;
using Avalonia.Data.Core;
using Avalonia.Markup.Parsers;
using Avalonia.Markup.Xaml.Parsers;
using Avalonia.Utilities;
using Xunit;

namespace Avalonia.Markup.Xaml.UnitTests.Parsers
{
    public class PropertyParserTests
    {
        [Fact]
        public void Parses_Name()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("Foo");
            var (ns, owner, name) = target.Parse(reader);

            Assert.Null(ns);
            Assert.Null(owner);
            Assert.Equal("Foo", name);
        }

        [Fact]
        public void Parses_Owner_And_Name()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("Foo.Bar");
            var (ns, owner, name) = target.Parse(reader);

            Assert.Null(ns);
            Assert.Equal("Foo", owner);
            Assert.Equal("Bar", name);
        }

        [Fact]
        public void Parses_Namespace_Owner_And_Name()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("foo:Bar.Baz");
            var (ns, owner, name) = target.Parse(reader);

            Assert.Equal("foo", ns);
            Assert.Equal("Bar", owner);
            Assert.Equal("Baz", name);
        }

        [Fact]
        public void Parses_Owner_And_Name_With_Parentheses()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("(Foo.Bar)");
            var (ns, owner, name) = target.Parse(reader);

            Assert.Null(ns);
            Assert.Equal("Foo", owner);
            Assert.Equal("Bar", name);
        }

        [Fact]
        public void Parses_Namespace_Owner_And_Name_With_Parentheses()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("(foo:Bar.Baz)");
            var (ns, owner, name) = target.Parse(reader);

            Assert.Equal("foo", ns);
            Assert.Equal("Bar", owner);
            Assert.Equal("Baz", name);
        }

        [Fact]
        public void Fails_With_Empty_String()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(0, ex.Column);
            Assert.Equal("Expected property name.", ex.Message);
        }

        [Fact]
        public void Fails_With_Only_Whitespace()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("  ");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(0, ex.Column);
            Assert.Equal("Unexpected ' '.", ex.Message);
        }

        [Fact]
        public void Fails_With_Leading_Whitespace()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader(" Foo");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(0, ex.Column);
            Assert.Equal("Unexpected ' '.", ex.Message);
        }

        [Fact]
        public void Fails_With_Trailing_Whitespace()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("Foo ");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(3, ex.Column);
            Assert.Equal("Unexpected ' '.", ex.Message);
        }

        [Fact]
        public void Fails_With_Invalid_Property_Name()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("123");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(0, ex.Column);
            Assert.Equal("Unexpected '1'.", ex.Message);
        }

        [Fact]
        public void Fails_With_Trailing_Junk()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("Foo%");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(3, ex.Column);
            Assert.Equal("Unexpected '%'.", ex.Message);
        }

        [Fact]
        public void Fails_With_Invalid_Property_Name_After_Owner()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("Foo.123");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(4, ex.Column);
            Assert.Equal("Unexpected '1'.", ex.Message);
        }

        [Fact]
        public void Fails_With_Whitespace_Between_Owner_And_Name()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("Foo. Bar");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(4, ex.Column);
            Assert.Equal("Unexpected ' '.", ex.Message);
        }

        [Fact]
        public void Fails_With_Too_Many_Segments()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("Foo.Bar.Baz");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(8, ex.Column);
            Assert.Equal("Unexpected '.'.", ex.Message);
        }

        [Fact]
        public void Fails_With_Too_Many_Namespaces()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("foo:bar:Baz");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(8, ex.Column);
            Assert.Equal("Unexpected ':'.", ex.Message);
        }

        [Fact]
        public void Fails_With_Parens_But_No_Owner()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("(Foo)");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(1, ex.Column);
            Assert.Equal("Expected property owner.", ex.Message);
        }

        [Fact]
        public void Fails_With_Parens_And_Namespace_But_No_Owner()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("(foo:Bar)");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(1, ex.Column);
            Assert.Equal("Expected property owner.", ex.Message);
        }

        [Fact]
        public void Fails_With_Missing_Close_Parens()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("(Foo.Bar");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(8, ex.Column);
            Assert.Equal("Expected ')'.", ex.Message);
        }

        [Fact]
        public void Fails_With_Unexpected_Close_Parens()
        {
            var target = new PropertyParser();
            var reader = new CharacterReader("Foo.Bar)");

            var ex = Assert.Throws<ExpressionParseException>(() => target.Parse(reader));
            Assert.Equal(7, ex.Column);
            Assert.Equal("Unexpected ')'.", ex.Message);
        }
    }
}