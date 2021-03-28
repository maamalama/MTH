import React, { Component } from 'react';
import { Alert, AdaptivityProvider, AppRoot, ConfigProvider, PanelHeader, Root, View, Panel, FormLayout, FormLayoutGroup, FormItem, Input, Button, Spinner, CustomSelect, CardGrid, Card, Group, Cell, Radio, ModalPage, ModalRoot, ModalPageHeader, ScreenSpinner, Header, SimpleCell, Div, Title, Textarea } from '@vkontakte/vkui';
import { io } from "socket.io-client"
import '@vkontakte/vkui/dist/vkui.css';
import './css/style.css';
import vkQr from '@vkontakte/vk-qr';
import { send } from './server_api';
import { insideBoundingBox, getBoundingBox } from 'geolocation-utils';
import './style.css'

const locations1 = [
	{ lat:55.7172559, lon: 37.562876 },
  ]

  const locations2 = [
	{ lat:55.7175200, lon: 37.563694 },
  ]

class App extends Component {
	state = {
		popout: null,
		snackbar: null,
		answers: null,
		activePanelAdmin: "create_user",
		activePanelUser: "main",
		activeView: "user",
		user_id: "",
		user: {
			name: null,
			sex: null,
			age: null
		},
		dataQuestions: null
	}

	setActiveModal = (activeModal) => {
		this.setState({ activeModal })
	}




	componentDidMount() {
		const socket = new io("wss://81.177.165.209:3000", { transports: ["websocket"]})
		socket.onAny((d) => console.log(d))
		if (window.location.hash === "#admin") {
			this.setState({ activeView: "admin", activePanelAdmin: "create_user" })

		} else {
			this.setState({ user_id: window.location.hash.split("user")[1], socket })
			this.getGeolcation(); 
			send("quest", {}).then(data => {
				this.setState({ dataQuestions: data.question })
			})
		}


	}

	getGeolcation = () => {
				navigator.geolocation.watchPosition((pos) => {
					this.state.socket.emit("geo", { user_id: this.state.user_id, coords:pos.coords})
						console.log(pos)
						console.log(insideBoundingBox({ latitude: pos.coords.latitude, longitude: pos.coords.longitude }, getBoundingBox(locations1, 1)) ? "1" : "");
						console.log(insideBoundingBox({ latitude: pos.coords.latitude, longitude: pos.coords.longitude }, getBoundingBox(locations2, 1)) ? "2" : "");
					//send("coor-users", { user_id: this.state.user_id, lat: position.coords.latitude, lon: position.coords.longitude, number: 1 });
				}), ((err) => {
					this.getGeolcation();
					this.setPopout(
						<Alert
							header="Вы запретили геолокацию"
							text="Для нормальной работы сервиса разрешите геолокацию"
							onClose={() => this.setPopout(null)}
						>
	
						</Alert>)
				})
	}

	setPopout = (popout) => {
		this.setState({ popout });
	}

	createQR = () => {
		this.setPopout(<ScreenSpinner />)
		send("user", this.state.user).then(data => {
			const qrSvg = vkQr.createQR("https://best11-mice.surge.sh/#user" + data.user_id, {
				qrSize: 256,
				isShowLogo: false,
				className: "QR-container__qr-code"
			});
			document.querySelector("#QR_container").innerHTML = qrSvg;
			this.setPopout(null)
		});
	}

	onChange = (e) => {
		const { name, value } = e.target;
		this.setState({ user: { ...this.state.user, [name]: value } });
	}


	showQuestion = (question) => {
		this.setActiveModal("questions");
		this.setState({ question })
	}

	
	render() {
		const { activePanel, activeView, user, popout, activeModal, question, activePanelAdmin, activePanelUser } = this.state;
		return (
			<ConfigProvider>
				<AdaptivityProvider>
					<AppRoot>
						<Root popout={popout} activeView={activeView} modal={
							<ModalRoot activeModal={activeModal}>
								<ModalPage id="questions" header={<ModalPageHeader>Вопрос</ModalPageHeader>} onClose={() => this.setState({ activeModal: null, current_answer_id: null })}>
									{question &&
										<div>
											<Title style={{ marginLeft: 16, paddingBottom: 8 }} level="2">{question.name}</Title>
											{
												question.questions_answer.map((el, index) => {
													return (
														<Radio onChange={() => this.setState({ current_answer_id: el.id })} name="answer">{el.answer.name}</Radio>
													)

												})
											}

												<Div>
													<Button disabled={!this.state.current_answer_id} stretched onClick={() => {
														this.setActiveModal(null)
														send("answer-user", { user_id: this.state.user_id, questions_answer_id: this.state.current_answer_id })
														let dataQuestions = this.state.dataQuestions;
														let i = dataQuestions.findIndex(i => i.id == question.id);
														if(this.state.current_answer_id === 13 || this.state.current_answer_id == 14) {
															this.setActiveModal("share")
														}
														
														//this.setState({ dataQuestions, current_answer_id: null });
															dataQuestions.splice(i, 1);
															this.setState({ dataQuestions, current_answer_id: null })
														//this.setState({ dataQuestions, current_answer_id: null })
													}} size="l" style={{ backgroundColor: "#97EDAA" }}>Ответить</Button>
												</Div>
											
										</div>
									}
								</ModalPage>

								<ModalPage id="share" header={<ModalPageHeader>Ваше мнение</ModalPageHeader>} onClose={() => this.setActiveModal(null)}>
									<Div>
										<Textarea onChange={(e) => this.setState({ comment: e.target.value })} />
									</Div>
									<Div>
										<Button size="l" stretched onClick={() => {
											send("user-last-update", { user_id: this.state.user_id, comment: this.state.comment});
											this.setActiveModal(null)
										}} style={{ backgroundColor: "#97EDAA" }}>Отправить</Button>
									</Div>
								</ModalPage>
							</ModalRoot>
						}>
							<View activePanel={activePanelAdmin} id="admin">
								<Panel id="create_user">
									<PanelHeader>
										Мы подружимся
									</PanelHeader>
									<FormLayout>
										<FormLayoutGroup>
											<FormItem top="Как к вам обращаться?">
												<Input name="name" onChange={this.onChange} value={user.name} type="text" />

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
												this.state.dataQuestions && this.state.dataQuestions.map((el, index) => {
													return (
														<Card className={el.animation ? "ball duration-10000" : null} id={el.id} onClick={() => this.showQuestion(el)} className="question">
															<div className="question__content">
																
																<div className="question__content-text">{el.name}</div>
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

