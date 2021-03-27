import React, { Component } from 'react';
import { AdaptivityProvider, AppRoot, ConfigProvider, PanelHeader, Root, View, Panel, FormLayout, FormLayoutGroup, FormItem, Input, Button, Spinner, CustomSelect, CardGrid, Card, Group } from '@vkontakte/vkui';
import { io } from "socket.io-client"
import '@vkontakte/vkui/dist/vkui.css';
import './css/style.css';
import vkQr from '@vkontakte/vk-qr';
import { send } from './server_api';


class App extends Component {

	state = {
		popout: null,
		snackbar: null,
		activePanel: "main",
		activeView: "user",
		user: {
			name: null,
			sex: null,
			age: null
		},
		dataQuestions: ["Вопрос", "Вопросик", "Еще один"]
	}





	componentDidMount() {
		if (window.location.hash === "#admin") {
			console.log("data");
		} else {
			const socket = io("wss://195.161.62.85:3000", { transports: ["websocket"], autoConnect: false });
			socket.open();
			this.setState({ socket });
			this.getGeolcation();
		}


	}

	getGeolcation = () => {
		if ("geolocation" in navigator) {
			navigator.geolocation.watchPosition((position) => {
				console.log(position)
				this.state.socket.emit("geo", { user_id: 1, coords: position.coords });
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
		let user_id = 0;

		send("user", this.state.user).then(data => {
			const qrSvg = vkQr.createQR("data.user_id", {
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



	render() {
		const { activePanel, activeView, user, poput } = this.state;
		return (
			<ConfigProvider>
				<AdaptivityProvider>
					<AppRoot>
						<Root activeView={activeView}>
							<View id="admin">
								<Panel id="create_user">
									<PanelHeader>
										Админ
									</PanelHeader>
									<FormLayout>
										<FormLayoutGroup>
											<FormItem top="ФИО">
												<Input name="name" onChange={this.onChange} value={user.name} type="text" />
											</FormItem>
											<FormItem top="Пол">
												<CustomSelect placeholder="Не выбрано"></CustomSelect>
												<Input name="sex" onChange={this.onChange} value={user.sex} type="text" />
											</FormItem>
											<FormItem top="Возраст">
												<Input name="age" onChange={this.onChange} value={user.age} type="number" />
											</FormItem>
											<FormItem>
												<Button onClick={() => this.createQR()} size="l" mode="commerce" stretched >QR-код</Button>
											</FormItem>
										</FormLayoutGroup>
									</FormLayout>
									<div id="QR_container" className="QR-container"></div>
								</Panel>
							</View>

							<View id="user" activePanel={activePanel}>
								<Panel id="main">
									<PanelHeader>
										Вопросы
									</PanelHeader>
									<Group>
										<CardGrid size="l">
											{
												this.state.dataQuestions.map((el, index) => {
													return (
														<Card className="question">
															<div className="question__content">
																<span className="question__content-index">{`${index +1 } `}</span>
																<div class="question__content-text">{el}</div>
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

