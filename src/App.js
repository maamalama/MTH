import React, { Component } from 'react';
import bridge from '@vkontakte/vk-bridge';
import { AdaptivityProvider, AppRoot, ConfigProvider, PanelHeader, Root, View, Panel, FormLayout, FormLayoutGroup, FormItem, Input, Button, Spinner, CustomSelect } from '@vkontakte/vkui';
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
		if ("geolocation" in navigator) {
			navigator.geolocation.watchPosition((position) => {
				console.log(position)
			});
		} else {
			/* местоположение НЕ доступно */
		}
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
		this.setState({ user: { ...this.state.user, [name] : value} });
		
	}


	render() {
		const { activePanel, activeView, user } = this.state;
		return (
			<ConfigProvider>
				<AdaptivityProvider>
					<AppRoot>
						<Root activeView={activeView}>
							<View id="admin" activePanel={activePanel}>
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
												<Input name="age" onChange={this.onChange} value={user.age} type="text" />
											</FormItem>
											<FormItem>
												<Button onClick={() => this.createQR()} size="l" mode="commerce" stretched >QR-код</Button>
											</FormItem>
										</FormLayoutGroup>
									</FormLayout>
									<div id="QR_container" className="QR-container"></div>
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

