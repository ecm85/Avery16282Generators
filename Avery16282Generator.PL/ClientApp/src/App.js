import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Brewcrafters } from './components/Brewcrafters';
import { SpiritIsland } from './components/SpiritIsland';
import { AeonsEnd } from './components/AeonsEnd';
import { Legendary } from './components/Legendary';
import { Dominion } from './components/Dominion';
import { ArkhamHorrorLCG } from './components/ArkhamHorrorLCG';
import { ErrorTest } from './components/ErrorTest';

export default class App extends Component {
  displayName = App.name

  render() {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/brewcrafters' component={Brewcrafters} />
        <Route path='/spiritisland' component={SpiritIsland} />
        <Route path='/aeonsend' component={AeonsEnd} />
        <Route path='/legendary' component={Legendary} />
        <Route path='/dominion' component={Dominion} />
        <Route path='/arkhamhorrorlcg' component={ArkhamHorrorLCG} />
        <Route path='/errorTest' component={ErrorTest} />
      </Layout>
    );
  }
}
