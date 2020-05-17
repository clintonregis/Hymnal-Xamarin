using System.Collections.Generic;
using System.Diagnostics;
using Hymnal.Core.Services;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Hymnal.Core.ViewModels
{
    public class RootViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService navigationService;
        private readonly IPreferencesService preferencesService;
        private readonly IAppInformationService appInformationService;

        public RootViewModel(
            IMvxNavigationService navigationService,
            IPreferencesService preferencesService,
            IAppInformationService appInformationService
            )
        {
            this.navigationService = navigationService;
            this.preferencesService = preferencesService;
            this.appInformationService = appInformationService;
        }

        private bool loaded = false;
        public override async void ViewAppearing()
        {
            base.ViewAppearing();

            if (loaded)
                return;

            loaded = true;

            await navigationService.Navigate<NumberViewModel>();
            await navigationService.Navigate<IndexViewModel>();
            await navigationService.Navigate<FavoritesViewModel>();
            await navigationService.Navigate<SettingsViewModel>();

            //await navigationService.Navigate<SimpleViewModel>();
        }

        // LifeCycle implemented in RootViewModel
        #region LifeCycle
        public override void Start()
        {
            Debug.WriteLine("App Started");

            Analytics.TrackEvent(Constants.TrackEvents.AppOpened, new Dictionary<string, string>
            {
                { Constants.TrackEvents.AppOpenedScheme.CultureInfo, Constants.CurrentCultureInfo.Name },
                { Constants.TrackEvents.AppOpenedScheme.HymnalVersion, preferencesService.ConfiguratedHymnalLanguage.Id },
                { Constants.TrackEvents.AppOpenedScheme.ThemeConfigurated, appInformationService.RequestedTheme.ToString() }
            });

            base.Start();
        }
        #endregion
    }
}
