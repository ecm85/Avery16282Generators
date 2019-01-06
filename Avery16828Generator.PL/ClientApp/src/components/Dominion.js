import React, { Component } from 'react';

export class Dominion extends Component {
    displayName = Dominion.name

  constructor(props) {
    super(props);
      this.state = { filename: null, loading: false};
      this.generateDominion = this.generateDominion.bind(this);
  }

    generateDominion() {
        this.setState({ filename: null, loading: true });
        fetch('api/PdfGenerator/GenerateDominion')
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
        <h1>Dominion</h1>
        <button onClick={this.generateDominion}>Generate Labels</button>
            {contents}
      </div>
    );
  }
}
