using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostalServiceApp.Models;
using PostalServiceApp.ViewModels;
using PostalServiceApp.ViewModels.DataForms;
using PostalServiceApp.Views;
using PostalServiceApp.Views.DataForms;
using PostalServiceClassLibrary.Models;

namespace PostalServiceApp.Services;

// Класс, обеспечивающий создание окон для ввода данных
public class InputDataService
{

	public bool? InputNewSubscribe(Subscribe subscribe, DataService dataService)
	{
		IDialogService dialog = new DialogService();
		ViewModelBase viewModel = new SubscribeFormViewModel(subscribe, dataService);
		return dialog.ShowDialog<SubscribeFormView>(viewModel);
	}


	public bool? InputNewPublication(Publication publication)
	{
		IDialogService dialog = new DialogService();
		ViewModelBase viewModel = new PublicationFormViewModel(publication);
		return dialog.ShowDialog<PublicationFormView>(viewModel);
	}

	public bool? EditPublication(Publication publication)
	{
		IDialogService dialog = new DialogService();
		ViewModelBase viewModel = new PublicationFormViewModel(publication, "Изменить данные об издании");
		return dialog.ShowDialog<PublicationFormView>(viewModel);
	}

	public bool? InputNewPostman(Postman postman)
	{
		IDialogService dialog = new DialogService();
		ViewModelBase viewModel = new PostmanFormViewModel(postman);
		return dialog.ShowDialog<PostmanFormView>(viewModel);
	}

	public bool? InputNewSubscriber(Subscriber subscriber)
	{
		IDialogService dialog = new DialogService();
		ViewModelBase viewModel = new SubscriberFormViewModel(subscriber);
		return dialog.ShowDialog<SubscriberFormView>(viewModel);
	}

	public bool? EditSubscriber(Subscriber subscriber)
	{
		IDialogService dialog = new DialogService();
		ViewModelBase viewModel = new SubscriberFormViewModel(subscriber, "Изменить данные о подписчике");
		return dialog.ShowDialog<SubscriberFormView>(viewModel);
	}

	public int? SelectingPostman(IEnumerable<PostmanInfo> postmans)
	{
		IDialogService dialog = new DialogService();
		var viewModel = new SelectPostmanViewModel(postmans);
			
		if (dialog.ShowDialog<SelectPostmanView>(viewModel) == true)
			return viewModel.SelectedPostman.Id;
			
		return null;
	}
}