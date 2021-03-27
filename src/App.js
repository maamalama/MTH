import React, { Component } from 'react';
import bridge from '@vkontakte/vk-bridge';
import { AdaptivityProvider, AppRoot, ConfigProvider, PanelHeader, Root, View, Panel } from '@vkontakte/vkui';
import '@vkontakte/vkui/dist/vkui.css';



class App extends Component {

	state = {
		popout: null, 
		snackbar: null,
		activePanel: "main",
		activeView: "main"
	}

	render() {
		const { activePanel, activeView } = this.state;
		return(
			<ConfigProvider>
				<AdaptivityProvider>
					<AppRoot>
						<Root activeView={activeView} modal={null}>
							<View id="main" activePanel={activePanel}>
								<Panel id="main">
									<PanelHeader>Чат</PanelHeader>
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

