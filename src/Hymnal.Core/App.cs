using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hymnal.Core.Extensions;
using Hymnal.Core.Models;
using Hymnal.Core.Models.Parameter;
using Hymnal.Core.Resources;
using Hymnal.Core.Services;
using Hymnal.Core.ViewModels;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Hymnal.Core
{
    public class App : MvxApplication
    {
        public static App Current;

        public App()
            : base()
        {
            Current = this;
        }


        public override void Initialize()
        {
            SetUp();

            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterAppStart<RootViewModel>();
        }

        private void SetUp()
        {
            IMultilingualService multilingualService = Mvx.IoCProvider.Resolve<IMultilingualService>();
            IPreferencesService preferencesService = Mvx.IoCProvider.Resolve<IPreferencesService>();
            IAppInformationService appInformationService = Mvx.IoCProvider.Resolve<IAppInformationService>();

            // Configurating language of the device
            Constants.CurrentCultureInfo = multilingualService.DeviceCultureInfo;
            AppResources.Culture = Constants.CurrentCultureInfo;

            // Configurating language of the hymnals
            if (preferencesService.ConfiguratedHymnalLanguage == null)
            {
                List<HymnalLanguage> lngs = Constants.HymnsLanguages.FindAll(l => l.TwoLetterISOLanguageName == Constants.CurrentCultureInfo.TwoLetterISOLanguageName);

                if (lngs.Count == 0)
                {
                    preferencesService.ConfiguratedHymnalLanguage = Constants.HymnsLanguages.First();
                }
                else
                {
                    preferencesService.ConfiguratedHymnalLanguage = lngs.First();
                }
            }

            // Configurate First Time opening this version
            if (preferencesService.LastVersionOpened != appInformationService.VersionString)
            {
                preferencesService.LastVersionOpened = appInformationService.VersionString;

                // Do somethink for new version

            }
        }


        public void LaunchPage<TViewModel>() where TViewModel : IMvxViewModel
        {
            IMvxNavigationService navigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            navigationService.Navigate<TViewModel>();
        }

        public void LaunchPage<TViewModel, TParameter>(TParameter parameter) where TViewModel : IMvxViewModel<TParameter>
        {
            IMvxNavigationService navigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            navigationService.Navigate<TViewModel, TParameter>(parameter);
        }

        public void PerformAppLinkRequest(Uri uri)
        {
            var request = uri.ToString().Replace(Constants.AppLink.UriBase, string.Empty);

            if (!string.IsNullOrEmpty(request))
            {
                if (request.Equals(PageRequest.Search.ToString()))
                    LaunchPage<SearchViewModel>();

                if (request.Equals(PageRequest.Records.ToString()))
                    LaunchPage<RecordsViewModel>();

                if (request.Contains(PageRequest.Hymn.ToString()))
                {
                    IPreferencesService preferencesService = Mvx.IoCProvider.Resolve<IPreferencesService>();

                    LaunchPage<HymnViewModel, HymnIdParameter>(new HymnIdParameter
                    {
                        Number = 22,
                        HymnalLanguage = preferencesService.ConfiguratedHymnalLanguage
                    });

                }
            }
        }

        public void PerformPageRequest(PageRequest pageRequest)
        {
            switch (pageRequest)
            {
                case PageRequest.Records:
                    LaunchPage<RecordsViewModel>();
                    break;

                case PageRequest.Search:
                    LaunchPage<SearchViewModel>();
                    break;

                default:
                    Debug.Write($"Imposible to perform: {pageRequest}");
                    break;
            }
        }

    }
}
