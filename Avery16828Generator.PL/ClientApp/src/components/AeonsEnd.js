import React, { Component } from 'react';

export class AeonsEnd extends Component {
    displayName = AeonsEnd.name

  constructor(props) {
    super(props);
      this.state = { filename: null, loading: false};
      this.generateAeonsEnd = this.generateAeonsEnd.bind(this);
  }

    generateAeonsEnd() {
        this.setState({ filename: null, loading: true });
        fetch('api/PdfGenerator/GenerateAeonsEnd')
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
        <h1>AeonsEnd</h1>
        <button onClick={this.generateAeonsEnd}>Generate Labels</button>
            {contents}
      </div>
    );
  }
}
