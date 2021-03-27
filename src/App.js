import React, { Component } from 'react';
import bridge from '@vkontakte/vk-bridge';
import { AdaptivityProvider, AppRoot, ConfigProvider, PanelHeader, Root, View, Panel, FormLayout, FormLayoutGroup, FormItem, Input, Button } from '@vkontakte/vkui';
import '@vkontakte/vkui/dist/vkui.css';


class App extends Component {

	state = {
		popout: null, 
		snackbar: null,
		activePanel: "admin",
		activeView: "admin"
	}

	render() {
		const { activePanel, activeView } = this.state;
		return(
			<ConfigProvider>
				<AdaptivityProvider>
					<AppRoot>
						<Root activeView="admin">
							<View id="admin" activePanel="main">
								<Panel id="main">
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
													<Button size="l" mode="commerce" stretched >QR код</Button>
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

