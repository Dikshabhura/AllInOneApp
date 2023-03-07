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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AllInOneApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ToDoListView : Page
    {
        private GraphServiceClient gc;
        private string listId = "AAMkADAwYzExZjBjLTAxY2QtNDc5OC1iNWI3LTAxZTg1MGNmMGY2ZgAuAAAAAAAVpdoKmHXYQr8TZqNt7h4QAQAHs8eE9g4dT4ytMjsDuRdFAAAAAAESAAA=";
        public ObservableCollection<Task> myTasks = new ObservableCollection<Task>();
        Symbol taskPriority;
        public ToDoListView()
        {
            this.InitializeComponent();

            // Get the Graph client from MainPage;
            gc = MainPage.graphClient;

            GetMyTasks();
            //AddTask();
        }

        private async void GetMyTasks()
        {
            try
            {
                var mytodolist = await gc.Me.Todo.Lists[listId].Tasks.GetAsync();

                //Show only Not started or Incomplete task.
                if(mytodolist != null || mytodolist.Value.Count > 0) {
                    for (int index = 0; index <= mytodolist.Value.Count; index++)
                    {
                        var currTask = mytodolist.Value[index];
                        if (currTask.Status != TaskStatus.Completed)
                        {
                            myTasks.Add(new Task
                            {
                                Title = currTask.Title,
                                Id = currTask.Id,
                                Importance = currTask.Importance,
                                DueDateTime = currTask.DueDateTime!= null ? currTask.DueDateTime.ToString(): null,
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

        private void AddTask(object sender, RoutedEventArgs e)
        {
            try
            {
                string taskTitle = this.AddTaskTitle.Text;
                //var date = new DateTime(
                DateTimeOffset taskDate = (DateTimeOffset)this.taskDueDate.Date;
                

                DateTimeTimeZone dt = new DateTimeTimeZone();
                //dt.DateTime = new DateTime(
                

                var reqBody = new TodoTask
                {
                    Title = taskTitle,
                    //DueDateTime = 
                };

                var result = gc.Me.Todo.Lists[listId].Tasks.PostAsync(reqBody);

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
                    myTasks.Remove(selectedTask);
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
    }
}