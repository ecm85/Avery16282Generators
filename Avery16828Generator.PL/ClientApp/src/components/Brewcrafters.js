import React, { Component } from 'react';

export class Brewcrafters extends Component {
    displayName = Brewcrafters.name

  constructor(props) {
    super(props);
      this.state = { filename: null, loading: false};
      this.generateBrewcrafters = this.generateBrewcrafters.bind(this);
  }

    generateBrewcrafters() {
        this.setState({ filename: null, loading: true });
        fetch('api/PdfGenerator/GenerateBrewcrafters')
            .then(response =>
                response.text())
            .then(data => {
                this.setState({ filename: data, loading: false });
            });
    }
 
    render() {
    let contents = this.state.loading
        ? <p><em>Loading...</em></p>
    : <div>{this.state.filename}</div>;
    return (
      <div>
        <h1>Brewcrafters</h1>
        <button onClick={this.generateBrewcrafters}>Generate Labels</button>
            {contents}
      </div>
    );
  }
}
