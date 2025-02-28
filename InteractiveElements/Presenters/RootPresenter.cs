using System.Linq;
using UnityEngine;

namespace CherryUI.InteractiveElements.Presenters
{
    public class RootPresenter : PresenterBase
    {
        [SerializeField] private PresenterLoadingBase loadingScreen;
        [SerializeField] private PresenterErrorBase errorScreen;

        public PresenterLoadingBase LoadingScreen => loadingScreen;
        public PresenterErrorBase ErrorScreen => errorScreen;

        private void Start()
        {
            childPresenters.Add(loadingScreen);
            childPresenters.Add(errorScreen);
            ViewService.PopView(childPresenters.First(), UiThis);
        }
    }
}