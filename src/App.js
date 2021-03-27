import React, { Component } from 'react';
import { io } from "socket.io-client"
import { AdaptivityProvider, AppRoot, ConfigProvider, PanelHeader, Root, View, Panel, FormLayout, FormLayoutGroup, FormItem, Input, Button, Alert } from '@vkontakte/vkui';
import '@vkontakte/vkui/dist/vkui.css';
import './css/style.css';
import vkQr from '@vkontakte/vk-qr';
import { send } from './server_api';


class App extends Component {

	state = {
		popout: null, 
		snackbar: null,
		activePanel: "create_user",
		activeView: "admin",
		user: {
			name: null,
			sex: null,
			age: null
		}
	}

	componentDidMount() {
		if(window.location.hash === "#admin") {
			console.log("data");
		} else {
			const socket = io("wss://195.161.62.85:3000" , { transports: ["websocket"], autoConnect: false } );
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
		const qrSvg = vkQr.createQR("user_id", {
			qrSize: 256,
			isShowLogo: false,
			className: "QR-container__qr-code"
		  });
			document.querySelector("#QR_container").innerHTML = qrSvg;
	}


	render() {
		const { activePanel, activeView, popout } = this.state;
		return(
			<ConfigProvider>
				<AdaptivityProvider>
					<AppRoot>
						<Root activeView={activeView}>
							<View id="admin" activePanel={activePanel} popout={popout}>
								<Panel id="create_user">
									<PanelHeader>
										Админ
									</PanelHeader>
										<FormLayout>
											<FormLayoutGroup>
												<FormItem top="ФИО">
													<Input name="name" type="text" />
												</FormItem>
												<FormItem top="Пол">
													<Input name="sex" type="text" />
												</FormItem>
												<FormItem top="Возраст">
													<Input name="age" type="text" />
												</FormItem>
												<FormItem>
													<Button onClick={this.createQR}  size="l" mode="commerce" stretched >QR-код</Button>
												</FormItem>
											</FormLayoutGroup>
										</FormLayout>
										<div id="QR_container" class="QR-container">

										</div>
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

