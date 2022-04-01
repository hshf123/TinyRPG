using System;
using System.Collections.Generic;
using System.Text;
using Server.Game.Job;
using ServerCore;

namespace Server.Game
{
	struct JobTimerElem : IComparable<JobTimerElem>
	{
		public int execTick; // 실행 시간
		public IJob job;

		public int CompareTo(JobTimerElem other)
		{
			return other.execTick - execTick;
		}
	}

	public class JobTimer
	{
		PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();
		object _lock = new object();

		public void Push(IJob job, int tickAfter = 0)
		{
			JobTimerElem jte;
			jte.execTick = System.Environment.TickCount + tickAfter;
			jte.job = job;

			lock (_lock)
			{
				_pq.Push(jte);
			}
		}

		public void Flush()
		{
			while (true)
			{
				int now = System.Environment.TickCount;

				JobTimerElem jte;

				lock (_lock)
				{
					if (_pq.Count == 0)
						break;

					jte = _pq.Peek();
					if (jte.execTick > now)
						break;

					_pq.Pop();
				}

				jte.job.Execute();
			}
		}
	}
}
