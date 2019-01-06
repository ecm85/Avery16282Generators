import React, { Component } from 'react';

export class Legendary extends Component {
    displayName = Legendary.name

  constructor(props) {
    super(props);
      this.state = { filename: null, loading: false};
      this.generateLegendary = this.generateLegendary.bind(this);
  }

    generateLegendary() {
        this.setState({ filename: null, loading: true });
        fetch('api/PdfGenerator/GenerateLegendary')
            .then(response =>
                response.text())
            .then(data => {
                this.setState({ filename: data, loading: false });
            });
    }
 
    render() {
    let contents = this.state.loading ?
        <p><em>Creating...</em></p> :
        this.state.filename === null ?
            <div></div> : 
            <div><a download href={"api/PdfGenerator/GetFile?fileName=" + this.state.filename}>Download PDF</a></div>
    return (
      <div>
        <h1>Legendary</h1>
        <button onClick={this.generateLegendary}>Generate Labels</button>
            {contents}
      </div>
    );
  }
}
