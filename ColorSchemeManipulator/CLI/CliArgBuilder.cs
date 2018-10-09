using System;
using System.Collections.Generic;
using ColorSchemeManipulator.Colors;
using ColorSchemeManipulator.Filters;

namespace ColorSchemeManipulator.CLI
{
    public sealed class CliArgBuilder
    {
        private readonly List<string> _optionArgs = new List<string>();
        private FilterWrapper _filter;
        private byte _minParams;
        private byte _maxParams;
        private string _description = "";
        private string _paramList = "";
        private string _paramDesc = "";

        public CliArgBuilder() { }

        public CliArgBuilder AddOption(string option)
        {
            _optionArgs.Add(option);
            return this;
        }

        public CliArgBuilder AddOptions(params string[] options)
        {
            foreach (var option in options) {
                _optionArgs.Add(option);
            }

            return this;
        }

        public CliArgBuilder Filter(Func<Color, ColorRange, double[], Color> singleFilter)
        {
            _filter = new FilterWrapper(singleFilter);
            return this;
        }
        
        public CliArgBuilder Filter(Func<IEnumerable<Color>, ColorRange, double[], IEnumerable<Color>> multiFilter)
        {
            _filter = new FilterWrapper(multiFilter);
            return this;
        }

        public CliArgBuilder Params(byte value)
        {
            _minParams = value;
            _maxParams = value;
            return this;
        }

        public CliArgBuilder Params(byte min, byte max)
        {
            _minParams = min;
            _maxParams = max;
            return this;
        }

        public CliArgBuilder Description(string value)
        {
            _description = value;
            return this;
        }

        public CliArgBuilder ParamString(string value)
        {
            _paramList = value;
            return this;
        }

        public CliArgBuilder ParamDescription(string value)
        {
            _paramDesc = value;
            return this;
        }

        // This is alternative for using Build() method as last piece of fluent build process

        public static implicit operator CliArg(CliArgBuilder cab)
        {
            return new CliArg(
                cab._optionArgs,
                cab._filter,
                cab._minParams,
                cab._maxParams,
                cab._description,
                cab._paramList,
                cab._paramDesc);
        }
    }
}