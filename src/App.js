import React, { Component } from 'react';
import bridge from '@vkontakte/vk-bridge';
import { AdaptivityProvider, AppRoot, ConfigProvider, Root } from '@vkontakte/vkui';
import '@vkontakte/vkui/dist/vkui.css';

import Home from './panels/Home';
import Persik from './panels/Persik';
import { Epic } from '@vkontakte/vkui/dist/components/Epic/Epic';
import { useStructure } from '@unexp/router';

class App extends Component {

	render() {
		const structure = useStructure({ view: 'home', panel: 'main' });
		return(
			<ConfigProvider>
				<AdaptivityProvider>
					<AppRoot>
						<Root popout={structure.popout} modal={null}>
							<Epic activeStory={structure.view}>
								
							</Epic>
						</Root>
					</AppRoot>
				</AdaptivityProvider>
			</ConfigProvider>
		)
	}
}

export default App;

