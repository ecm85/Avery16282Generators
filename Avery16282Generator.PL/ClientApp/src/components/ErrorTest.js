import React, { Component } from 'react';

export class ErrorTest extends Component {
    displayName = ErrorTest.name

  constructor(props) {
    super(props);
      this.state = { };
      this.throwException = this.throwException.bind(this);
      this.return500 = this.return500.bind(this);
  }

    throwException() {
        fetch('api/ErrorTest/ThrowException');
    }

    return500() {
        fetch('api/ErrorTest/Return500');
    }
 
    render() {
    return (
      <div>
        <h1>Error Test</h1>
        <button onClick={this.throwException}>Throw Exception</button>
        <button onClick={this.return500}>Return 500</button>
      </div>
    );
  }
}
