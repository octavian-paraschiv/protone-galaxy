using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Core.Logging;
using System.Diagnostics;

namespace OPMedia.Core
{
    public class TicToc : IDisposable
    {
        private Stopwatch _sw = new Stopwatch();

        string _opName = "";

        double _totalTime = 0;
        long _totalCount = 0;
        object _syncRoot = new object();
        
        int _avgReportCount = 20;
        long _longOpThreshold = 10;

        public TicToc(string opName, long longOpThreshold = 10, int avgReportCount = 20)
        {
            _opName = opName;
            _avgReportCount = avgReportCount;
            _longOpThreshold = longOpThreshold;

            Tic();
        }

        public void Tic()
        {
            _sw.Restart();
        }

        public long Toc()
        {
            return TocInternal(false);
        }

        private long TocInternal(bool disposing)
        {
            _sw.Stop();
            long diff = _sw.ElapsedMilliseconds;

            if (diff > _longOpThreshold)
                Logger.LogTrace($"Last \"{_opName}\" operation took {diff:0.000} msec");

            lock (_syncRoot)
            {
                _totalTime += diff;
                _totalCount++;
                double avg = _totalTime / _totalCount;

                if (disposing || _totalCount % _avgReportCount == 0)
                    Logger.LogTrace($"\"{_opName}\" operation takes {diff:0.000} msec in average");
            }

            return diff;
        }

        public void Dispose()
        {
            TocInternal(true);
        }
    }
}
