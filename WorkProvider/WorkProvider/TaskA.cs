using WorkProvider.Infrastructrue;
using WorkProvider.Services;
using System;
using System.Threading.Tasks;

namespace WorkProvider
{
    public class WorkTaskA : IWork
    {
        private readonly DateTime _created_;
        private readonly String _name_;

        public WorkTaskA(DateTime created, String name)
        {
            _created_ = created;
            _name_ = name;
        }

        public override String ToString()
        {
            return $"Work {_name_}";
        }

        public async Task Execute()
        {
            CustomLogger.Log(new String[] {
                $"{this.ToString()}",
                $"created at {_created_.ToString("yyyy-mm-dd hh:mm:ss")} ",
                $"was executed at {DateTime.UtcNow.ToString("yyyy-mm-dd hh:mm:ss")}"
            });
        }
    }

    public class WorkDequeuerTaskA : IWorkDequeuer
    {
        private readonly DateTime _started_;
        private Int32 _worksExecutedCounter_ = 0;
        private readonly IWork workAfter05Seconds = new WorkTaskA(DateTime.UtcNow, "05");
        private readonly IWork workAfter10Seconds = new WorkTaskA(DateTime.UtcNow, "10");
        private readonly IWork workAfter15Seconds = new WorkTaskA(DateTime.UtcNow, "15");
        private readonly IWork workAfter20Seconds = new WorkTaskA(DateTime.UtcNow, "20");

        public WorkDequeuerTaskA()
        {
            _started_ = DateTime.UtcNow;
        }

        public async Task<IWork> DequeueAsync()
        {

            if (DateTime.UtcNow - _started_ >= TimeSpan.FromSeconds(5) && _worksExecutedCounter_ == 0)
            {
                _worksExecutedCounter_++;
                return workAfter05Seconds;
            }
            else if (DateTime.UtcNow - _started_ >= TimeSpan.FromSeconds(10) && _worksExecutedCounter_ == 1)
            {
                _worksExecutedCounter_++;
                return workAfter10Seconds;
            }
            else if (DateTime.UtcNow - _started_ >= TimeSpan.FromSeconds(15) && _worksExecutedCounter_ == 2)
            {
                _worksExecutedCounter_++;
                return workAfter15Seconds;
            }
            else if (DateTime.UtcNow - _started_ >= TimeSpan.FromSeconds(20) && _worksExecutedCounter_ == 3)
            {
                _worksExecutedCounter_++;
                return workAfter20Seconds;
            }
            else
            {
                return null;
            }
        }
    }
}
