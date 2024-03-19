using System.Collections.Concurrent;

namespace ConsumerAT.MiddleWare
{
    public class TokenBucket
    {
        private BlockingCollection<Token> _tokens;
        private System.Timers.Timer _timer;
        private int _maxTokens;
        private DateTime _lastEventTime;

        public TokenBucket(int maxNumberOfTokens, int refillRateInMilliseconds)
        {
            _maxTokens = maxNumberOfTokens;
            _timer = new System.Timers.Timer(refillRateInMilliseconds);
            _tokens = new BlockingCollection<Token>(maxNumberOfTokens);
            Init();
        }

        public bool UseToken()
        {           
            return _tokens.TryTake(out Token _);
            
        }

        public double GetRemainingTime()
        {
            var timerInterval = _timer.Interval / 1000;
            var elapsedSinceLastEvent = (DateTime.Now - _lastEventTime).TotalSeconds;

            return ( timerInterval - elapsedSinceLastEvent);
        }

        private void Init()
        {           

            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Elapsed += OnTimerElapsed;
            populateTokens();
        }

        private void populateTokens()
        {          
            
            foreach (var newToken in Enumerable.Range(_tokens.Count, _maxTokens).Select(x => { return new Token(); }))
            {               
                _tokens.Add(newToken);
                _lastEventTime = DateTime.Now;
            }
        }

        private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {           
            if(_tokens.Count < _maxTokens)
            {               
                populateTokens();
            }                
        }
    }

    public record Token;
}
