import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Brewcrafters } from './components/Brewcrafters';
import { AeonsEnd } from './components/AeonsEnd';

export default class App extends Component {
  displayName = App.name

  render() {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/brewcrafters' component={Brewcrafters} />
        <Route path='/aeonsend' component={AeonsEnd} />
        <Route path='/fetchdata' component={FetchData} />
      </Layout>
    );
  }
}
