using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace Infornographyx.Todo {
    public class TaskListEditor : EditorWindow
    {
        VisualElement container;
        TextField addTaskText;
        Button addTaskButton;
        ScrollView taskListScrollView;
        ObjectField savedTasksObjectField;
        Button loadTasksButton;
        TaskListSO  taskListSO;
        Button  removeTaskButton;
        public const string path = "Assets/Todo/Editor/EditorWindow/";
        [MenuItem("Tools/ToDo")]
        public static void ShowWindow()
        {
            TaskListEditor window = GetWindow<TaskListEditor>();
            window.titleContent = new GUIContent("Task List");
        }
        void CreateGUI()
        {
            container = rootVisualElement;
            VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path + "TaskListEditor.uxml");
            container.Add(original.Instantiate());
        
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path + "TaskListEditor.uss");
            container.styleSheets.Add(styleSheet);

            savedTasksObjectField = container.Q<ObjectField>("savedTasksObjectField");
            savedTasksObjectField.objectType = typeof(TaskListSO);
            loadTasksButton = container.Q<Button>("loadTasksButton");
            loadTasksButton.clicked += LoadTasks;
            
            addTaskText = container.Q<TextField>("addTaskText");

            addTaskButton = container.Q<Button>("addTaskButton");
            addTaskButton.clicked += AddTask;

            taskListScrollView = container.Q<ScrollView>("taskList");
            removeTaskButton = container.Q<Button>("removeTaskButton");
            removeTaskButton.clicked += RemoveTask;

        }
        void AddTask()
        {
            if(!string.IsNullOrEmpty(addTaskText.value))
            {
                taskListScrollView.Add(CreateTask(addTaskText.value));
                SaveTask(addTaskText.value);
                addTaskText.value = "";
            }
        }
        Toggle CreateTask(string addTaskText)
        {
            Toggle taskItem = new Toggle();
            taskItem.text = addTaskText;
            return taskItem;
        }
        void LoadTasks()
        {
            taskListSO = savedTasksObjectField.value as TaskListSO;
            if(taskListSO != null)
            {
                taskListScrollView.Clear();
                List<string> tasks = taskListSO.GetTasks();
                foreach(string task in tasks)
                {
                    taskListScrollView.Add(CreateTask(task));
                }
            }
        }
        void SaveTask(string task)
        {
            taskListSO.AddTask(task);
            EditorUtility.SetDirty(taskListSO); 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        void RemoveTask()
        {
            if(taskListSO != null)
            {
                List<string> tasks = new List<string>();
                foreach(Toggle task in taskListScrollView.Children())
                {
                    if(!task.value)
                    {
                        tasks.Add(task.text);
                    }
                }
                taskListSO.AddTasks(tasks);
                EditorUtility.SetDirty(taskListSO); 
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                LoadTasks();
            }
        }  
    }
}
