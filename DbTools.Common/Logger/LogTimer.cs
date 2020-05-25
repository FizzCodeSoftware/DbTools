namespace FizzCode.DbTools.Common.Logger
{
    using System.Diagnostics;

    public class LogTimer
    {
        private readonly Stopwatch _sw;
        private readonly LogSeverity _severity;
        private readonly string _text;
        private readonly string _module;
        private readonly object[] _args;

        public LogTimer(Logger logger, LogSeverity severity, string text, string module, params object[] args)
        {
            _logger = logger;
            _severity = severity;
            _text = text;
            _module = module;
            _args = args;

            _sw = new Stopwatch();
            _sw.Start();
        }

        public LogTimer(Logger logger, string text, string module, params object[] args)
            : this(logger, LogSeverity.Verbose, text, module, args)
        {
        }

        private readonly Logger _logger;

        public void Done()
        {
            var _timerText = "Took {ms}: " + _text;
            _sw.Stop();
            _logger.Log(_severity, _timerText, _module, _sw.ElapsedMilliseconds, _args);
        }
    }
}