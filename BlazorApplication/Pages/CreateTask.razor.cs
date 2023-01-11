using BlazorApplication.HttpRepository;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazorApplication.Pages
{
	public partial class CreateTask
	{
		private Models.Task _task = new Models.Task();
		private string _timeFrameMode = "1";
		private Enums.TaskCategory _taskCategory = Enums.TaskCategory.Easy;
		private SuccessNotification _notification;
		private string _time { get; set; }


        [Inject]
		public ITaskHttpRepository TaskRepo { get; set; }

		private async void Create()
		{

			_task.TaskCategoryId = (int)_taskCategory;

            //switch (_timeFrameMode)
            //{
            //	case "1":
            //		_task.Timeframe = new TimeSpan(0, 30, 0);
            //		break;
            //	case "2":
            //		_task.Timeframe = new TimeSpan(1, 0, 0);
            //		break;
            //	case "3":
            //		_task.Timeframe = new TimeSpan(1, 30, 0);
            //		break;
            //	case "4":
            //		_task.Timeframe = new TimeSpan(2, 0, 0);
            //		break;
            //}

            if (_time.Contains('.'))
            {
                _time = _time.Replace('.', ':');
            }
            if (_time.Contains(':'))
            {
                string[] numbers = _time.Split(':');
                if (numbers.Length == 3)
                {

                    try
                    {
                        if (Convert.ToInt32(numbers[0]) < 25)
                        {
                            _task.Timeframe = new TimeSpan(Convert.ToInt32(numbers[0]), Convert.ToInt32(numbers[1]), Convert.ToInt32(numbers[2]));
                        }
                        else
                        {
                            return;
                        }

                    }
                    catch
                    {
                        return;
                    }

                }
                else if (numbers.Length == 2)
                {
                    try
                    {
                        if (Convert.ToInt32(numbers[0]) < 25)
                        {
                            _task.Timeframe = new TimeSpan(Convert.ToInt32(numbers[0]), Convert.ToInt32(numbers[1]), 0);
                        }
                        else
                        {
                            return;
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
            }
            else
            {
                try
                {
                    _task.Timeframe = new TimeSpan(0, Convert.ToInt32(_time), 0);
                }
                catch
                {
                    return;
                }
            }

            await TaskRepo.CreateTask(_task);
			_notification.Show();
		}
	}
}
