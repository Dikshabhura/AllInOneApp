using CommunityToolkit.Authentication;
using System;
using CommunityToolkit.Graph.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Collections.ObjectModel;
using AllInOneApp.Models;
using System.Threading.Tasks;
using Task = AllInOneApp.Models.Task;
using TaskStatus = Microsoft.Graph.Models.TaskStatus;
using AllInOneApp.Helper;
using Windows.UI.Notifications;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AllInOneApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ToDoListView : Page
    {
        DateTimeConversion dateTimeConversion = new DateTimeConversion();
        private GraphServiceClient gc;
        private string listId = string.Empty;
        //private string listId = "AAMkADAwYzExZjBjLTAxY2QtNDc5OC1iNWI3LTAxZTg1MGNmMGY2ZgAuAAAAAAAVpdoKmHXYQr8TZqNt7h4QAQAHs8eE9g4dT4ytMjsDuRdFAAAAAAESAAA=";
        public ObservableCollection<Task> myPendingTasks = new ObservableCollection<Task>();
        public ObservableCollection<Task> myCompletedTasks = new ObservableCollection<Task>();
        Symbol taskPriority;
        public ToDoListView()
        {
            this.InitializeComponent();

            // Get the Graph client from MainPage;
            gc = MainPage.graphClient;
            
            GetMyTasks();
            
        }

        private async Task<string> GetToDoTaskListId()
        {
            string taskListId = string.Empty;
            try
            {
                
                var todoList = await gc.Me.Todo.Lists.GetAsync();

                taskListId = todoList.Value.First(l => l.DisplayName == "Tasks").Id;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return taskListId;
        }

        private async void GetMyTasks()
        {
            try
            {
                listId = await GetToDoTaskListId();

                var mytodolist = await gc.Me.Todo.Lists[listId].Tasks.GetAsync();

                //Show only Not started or Incomplete task.
                if(mytodolist.Value != null || mytodolist.Value.Count > 0) {
                    for (int index = 0; index < mytodolist.Value.Count; index++)
                    {
                        var currTask = mytodolist.Value[index];
                        if (currTask.Status != TaskStatus.Completed)
                        {
                            myPendingTasks.Add(new Task
                            {
                                Title = currTask.Title,
                                Id = currTask.Id,
                                Importance = currTask.Importance,
                                DueDateTime = currTask.DueDateTime,
                                TaskPriority = currTask.Importance == Importance.High ? Symbol.Pin : Symbol.UnPin,
                            });
                        }
                        else
                        {
                            myCompletedTasks.Add(new Task
                            {
                                Title = currTask.Title,
                                Id = currTask.Id,
                                Importance = currTask.Importance,
                                DueDateTime = currTask.DueDateTime,
                                Status = TaskStatus.Completed,
                                TaskPriority = currTask.Importance == Importance.High ? Symbol.Pin : Symbol.UnPin,
                            });
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void AddTask(object sender, RoutedEventArgs e)
        {
            try
            {

                //string taskTitle = this.AddTaskTitle.Text;
                //var retrievedDate = taskDueDate.Date;
                //DateTime dateTime = retrievedDate.Value.DateTime;
                //string date = dateTime.ToString("yyyy-MM-dd");
                //string time = dateTime.ToString("hh:mm:ss");
                //var formatedtime = date + "T" + time;
                //System.Diagnostics.Debug.WriteLine(formatedtime);

                var reqBody = new TodoTask
                {
                    Title = this.AddTaskTitle.Text,
                    DueDateTime = new DateTimeTimeZone
                    {
                        DateTime = dateTimeConversion.DateTimeConverter(taskDueDate.Date.Value),
                        TimeZone = "UTC"
                    },
                };

                var result = await gc.Me.Todo.Lists[listId].Tasks.PostAsync(reqBody);

                myPendingTasks.Add(new Task
                {
                    Title = result.Title,
                    Id = result.Id,
                    Importance = result.Importance,
                    DueDateTime = result.DueDateTime,
                    TaskPriority = result.Importance == Importance.High ? Symbol.Pin : Symbol.UnPin,
                });

                AddTaskTitle.Text = "";
                taskDueDate.Date = null;

                Console.WriteLine(result);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void UpdateTask(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rb = e.OriginalSource as RadioButton;
                var selectedTask = rb.DataContext as Task;
                var taskId = selectedTask.Id;

                var reqBody = new TodoTask
                {
                    Status = TaskStatus.Completed,
                };
                var result = await gc.Me.Todo.Lists[listId].Tasks[taskId].PatchAsync(reqBody);

                if (result.Status == TaskStatus.Completed)
                {
                    myPendingTasks.Remove(selectedTask);

                    myCompletedTasks.Add(new Task
                    {
                        Title = selectedTask.Title,
                        Id = selectedTask.Id,
                        Importance = selectedTask.Importance,
                        DueDateTime = selectedTask.DueDateTime,
                        Status = TaskStatus.Completed,
                        TaskPriority = selectedTask.Importance == Importance.High ? Symbol.Pin : Symbol.UnPin,
                    });
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message); 
            }
        }

        private async void PinTask(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                SymbolIcon symbolIcon = sender as SymbolIcon;
                symbolIcon.Symbol = Symbol.Pin;

                var task = symbolIcon.DataContext as Task;
                var taskId = task.Id;

                task.Importance = task.Importance == Importance.High ? Importance.Normal : Importance.High;

                var reqBody = new TodoTask
                {
                    Importance = task.Importance,
                };
                var result = await gc.Me.Todo.Lists[listId].Tasks[taskId].PatchAsync(reqBody);

                
                task.TaskPriority = reqBody.Importance == Importance.High ? Symbol.Pin : Symbol.UnPin;
                symbolIcon.Symbol = reqBody.Importance == Importance.High ? Symbol.Pin : Symbol.UnPin;
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void SwitchTasks(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    this.MyTasks.ItemsSource = myCompletedTasks;
                }
                else
                {
                    this.MyTasks.ItemsSource = myPendingTasks;
                }
            }
            
        } 
        
    }
}