import React, { Component } from 'react';
import bridge from '@vkontakte/vk-bridge';
import { AdaptivityProvider, AppRoot, ConfigProvider, PanelHeader, Root, View, Panel, FormLayout, FormLayoutGroup, FormItem, Input, Button } from '@vkontakte/vkui';
import '@vkontakte/vkui/dist/vkui.css';
import vkQr from '@vkontakte/vk-qr';


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

	createQR = () => {
		const qrSvg = vkQr.createQR('Test', {
			qrSize: 256,
			isShowLogo: true
		  });
		console.log(this.state.user);
	}

	render() {
		const { activePanel, activeView } = this.state;
		return(
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
													<Input name="name" type="text" />
												</FormItem>
												<FormItem top="Пол">
													<Input name="sex" type="text" />
												</FormItem>
												<FormItem top="Возраст">
													<Input name="age" type="text" />
												</FormItem>
												<FormItem>
													<Button onClick={this.createQR}  size="l" mode="commerce" stretched >QR код</Button>
												</FormItem>
											</FormLayoutGroup>
										</FormLayout>
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

