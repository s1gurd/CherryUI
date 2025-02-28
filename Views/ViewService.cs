using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CherryUI.InteractiveElements.Presenters;
using UnityEngine;

namespace CherryUI.Views
{
    public class ViewService
    {
        private readonly PresenterBase _root;
        private PresenterLoadingBase _loadingScreen;
        private PresenterErrorBase _errorScreen;

        private readonly Stack<List<PresenterBase>> _history = new();

        public ViewService(RootPresenter root)
        {
            _loadingScreen = root.LoadingScreen;
            _errorScreen = root.ErrorScreen;
            _root = root;
        }

        public void PopLoadingView()
        {
            PopView(_loadingScreen);
        }

        public void PopErrorView(string title, string message)
        {
            PopView(_errorScreen);
            _errorScreen.SetError(title, message);
        }

        public T PopView<T>(PresenterBase mountingPoint = null, bool skipAnimation = false) where T : PresenterBase
        {
            return PopView(typeof(T), mountingPoint, skipAnimation) as T;
        }

        public PresenterBase PopView(string typeString, PresenterBase mountingPoint = null, bool skipAnimation = false)
        {
            var type = ViewUtils.GetPresenterType(typeString);
            if (type == null)
            {
                Debug.LogError($"[View Service] View of type: {typeString} not found! Aborting...");
                return null;
            }

            return PopView(type, mountingPoint, skipAnimation);
        }

        public PresenterBase PopView(Type type, PresenterBase mountingPoint = null, bool skipAnimation = false)
        {
            var parentPresenter = mountingPoint ? mountingPoint : _root; ;

            var newView = parentPresenter.ChildPresenters.FirstOrDefault(p => p && p.GetType() == type);

            if (newView is null)
            {
                Debug.LogError(
                    $"[View Service] View of type: {type.Name} not registered in View Container: {parentPresenter.gameObject.name} of {mountingPoint?.gameObject.name}! Aborting...",
                    mountingPoint ? mountingPoint.gameObject : null);

                return null;
            }

            return PopView(newView, mountingPoint, skipAnimation);
        }

        public virtual PresenterBase PopView(PresenterBase view, PresenterBase mountingPoint = null, bool skipAnimation = false)
        {

            if (_history.TryPeek(out var current) && current.Last() is IPopUp)
            {
                _history.Pop();
            }

            var parentPresenter = mountingPoint ? mountingPoint : _root;

            if (!parentPresenter.ChildPresenters.Any(p => p.Equals(view)))
            {
                Debug.LogError(
                    $"[View Service] View : {view.gameObject.name} is not registered in View Container: {parentPresenter.gameObject.name} of {mountingPoint?.gameObject.name}! Aborting...",
                    mountingPoint ? mountingPoint.gameObject : null);
            }

            PresenterBase newView;

            if (view.gameObject.scene.IsValid())
            {
                newView = view;
                newView.gameObject.SetActive(true);
            }
            else
            {
                var index = parentPresenter.ChildPresenters.IndexOf(view);
                newView = UnityEngine.Object.Instantiate(view, parentPresenter.ChildrenContainer.transform);
                parentPresenter.ChildPresenters[index] = newView;

                if (newView is PresenterErrorBase e)
                {
                    _errorScreen = e;
                }

                if (newView is PresenterLoadingBase l)
                {
                    _loadingScreen = l;
                }
            }

            var newPath = new List<PresenterBase>();

            if (mountingPoint)
            {
                mountingPoint.currentChild = newView;
                newPath.AddRange(mountingPoint.uiPath);
                newPath.Add(mountingPoint);
            }

            newView.uiPath = newPath;

            var historyItem = new List<PresenterBase>(newPath) { newView };

            if (newView.ChildrenContainer != null && newView.ChildPresenters.Count > 0)
            {
                var viewToPop = newView.currentChild != null ? newView.currentChild : newView.ChildPresenters.First();
                historyItem.Add(PopView(viewToPop, newView, skipAnimation));
            }
            else
            {
                if (current != null && !current.Except(historyItem).ToList().Any() && !historyItem.Except(current).Any())
                {
                    DebugHistory("History duplicate");
                }
                else
                {
                    _history.Push(historyItem);
                    DebugHistory("History push");
                }
            }

            newView.ShowFrom(current?.Last(), skipAnimation);

            return newView;
        }

        public void ClearHistory()
        {
            var current = _history.Pop();
            //foreach (var item in _history)
            //{
            //    item.First().gameObject.SetActive(false);
            //}
            _history.Clear();
            _history.Push(current);
            DebugHistory("History clear");
        }

        public virtual void Back()
        {
            if (_history.Count < 2) return;

            var current = _history.Pop();
            DebugHistory("History back");

            if (!_history.TryPeek(out var path)) return;

            var lastItem = path.Last();

            for (var i = 0; i < path.Count; i++)
            {
                path[i].transform.SetAsLastSibling();
                path[i].gameObject.SetActive(true);
                if (current.Count == i + 1)
                {
                    current[i].transform.SetAsLastSibling();
                    lastItem = path[i];
                }
            }

            current.Last().HideTo(lastItem);
        }

        private void DebugHistory(string msg)
        {
            var sb = new StringBuilder();
            sb.Append($"[View Service] {msg}:\n");
            foreach (var item in _history)
            {
                sb.Append("#");
                foreach (var element in item)
                {
                    sb.Append(element.name);
                    sb.Append("/");
                }

                sb.Append("\n");
            }

            Debug.Log(sb.ToString());
        }
    }
}