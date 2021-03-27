import React, { Component } from 'react';
import { Alert, AdaptivityProvider, AppRoot, ConfigProvider, PanelHeader, Root, View, Panel, FormLayout, FormLayoutGroup, FormItem, Input, Button, Spinner, CustomSelect, CardGrid, Card, Group, Cell, Radio, ModalPage, ModalRoot, ModalPageHeader } from '@vkontakte/vkui';
import { io } from "socket.io-client"
import '@vkontakte/vkui/dist/vkui.css';
import './css/style.css';
import vkQr from '@vkontakte/vk-qr';
import { send } from './server_api';


if("serviceWorker" in navigator) {
    navigator.serviceWorker.register('./service-worker.js')
	.then((reg) => {
		console.log("success " + requestAnimationFrame.scope);
	}).catch((error) => {
		console.log("Error " + error);
	});
}

class App extends Component {
	state = {
		popout: null,
		snackbar: null,
		answers: null,
		activePanel: "main",
		activeView: "user",
		user_id: null,
		user: {
			name: null,
			sex: null,
			age: null
		},
		dataQuestions: [
			{
				text: "Как вам выставка?",
				id: "1",
				answers: ["ужасно 👎", "нормально 😐", "отлично 👍"]
			},
			{
				text: "Самый любимый экспонат",
				id: "2",
				answers: ["никакой", "который план эвакуации", "золотой стул"]
			},
			{
				text: "Придете еще?",
				id: "3", 
				answers: ["нет 🤢", "может быть 🤔", "да! 😊"]
			}

		]
	}

	setActiveModal = (activeModal) => {
		this.setState({ activeModal })
	}




	componentDidMount() {
		if (window.location.hash === "#admin") {
			this.setState({ activeView: "admin", activePanelAdmin: "create_user" })

		} else {
			this.setState({ user_id: window.location.hash.split("user")[1] })
			this.getGeolcation();
		}


	}

	getGeolcation = () => {
		if ("geolocation" in navigator) {
			navigator.geolocation.watchPosition((position) => {
				console.log(position)
				send("geo", { user_id: this.state.user_id, coords: position.coords });
			}, (err) => {
				this.getGeolcation();
				this.setPopout(
					<Alert
						header="Вы запретили геолокацию"
						text="Для нормальной работы сервиса разрешите геолокацию"
						onClose={() => this.setPopout(null)}
					>

					</Alert>)
			});
		} else {
			this.setPopout(
				<Alert
					header="выоваырлоафылалрфыв"
					text="Для нормальной работы сервиса разрешите геолокацию"
					onClose={() => this.setPopout(null)}
				>

				</Alert>)
		}
	}

	setPopout = (popout) => {
		this.setState({ popout });
	}

	createQR = () => {
		send("user", this.state.user).then(data => {
			const qrSvg = vkQr.createQR("https://user267319094-r7wx5wi4.wormhole.vk-apps.com/#user" + data.user_id, {
				qrSize: 256,
				isShowLogo: false,
				className: "QR-container__qr-code"
			});
			document.querySelector("#QR_container").innerHTML = qrSvg;
			console.log(data);
		});
	}

	onChange = (e) => {
		const { name, value } = e.target;
		this.setState({ user: { ...this.state.user, [name]: value } });
	}


	showQuestion = (answers) => {
		this.setActiveModal("questions");
		this.setState({ answers })
	}

	
	render() {
		const { activePanel, activeView, user, popout, activeModal, answers } = this.state;
		return (
			<ConfigProvider>
				<AdaptivityProvider>
					<AppRoot>
						<Root activeView={activeView} modal={
							<ModalRoot activeModal={activeModal}>
								<ModalPage id="questions" header={<ModalPageHeader>Вопросы</ModalPageHeader>} onClose={() => this.setState({ activeModal: null })}>
									{answers &&
										<div>
											{
												answers.map((el, index) => {
													return (
														<Radio name="answer">{el}</Radio>
													)

												})
											}
										</div>
									}
								</ModalPage>
							</ModalRoot>
						}>
							<View activePanel={activePanel} id="admin">
								<Panel id="create_user">
									<PanelHeader>
										Админ
									</PanelHeader>
									<FormLayout>
										<FormLayoutGroup>
											<FormItem top="ФИО">
												<Input name="name" onChange={this.onChange} value={user.name} type="text" required />

											</FormItem>
											<FormItem top="Пол">
												<Radio name="sex" value="женский" onChange={this.onChange}>Мужской</Radio>
												<Radio name="sex" value="мужской" onChange={this.onChange}>Женский</Radio>
												<Radio name="sex" value="другой" onChange={this.onChange}>Другой</Radio>
											</FormItem>

											<FormItem top="Дата рождения">
												<Input name="age" onChange={this.onChange} value={user.age} type="date" />
											</FormItem>
											<FormItem>
												<Button onClick={() => this.createQR()} size="l" mode="commerce" stretched >QR-код</Button>
											</FormItem>
										</FormLayoutGroup>
									</FormLayout>
									<div id="QR_container" className="QR-container"></div>
								</Panel>
							</View>

							<View id="user" activePanel={activePanelUser}>
								<Panel id="main">
									<PanelHeader>
										Вопросы
									</PanelHeader>
									<Group>
										<CardGrid size="l">
											{
												this.state.dataQuestions.map((el, index) => {
													return (
														<Card id={el.id} onClick={() => this.showQuestion(el.answers)} className="question">
															<div className="question__content">
																<span className="question__content-index">{`${index + 1} `}</span>
																<div class="question__content-text">{el.text}</div>
															</div>
														</Card>
													);
												})
											}
										</CardGrid>
									</Group>
								</Panel>
							</View>
						</Root>
					</AppRoot>
				</AdaptivityProvider>
			</ConfigProvider>
		)
	}
}

export default App;

