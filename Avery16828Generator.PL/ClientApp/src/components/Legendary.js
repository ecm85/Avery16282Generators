import React, { Component } from 'react';
//import Checkbox from "./Checkbox";

export class Legendary extends Component {
    displayName = Legendary.name

    constructor(props) {
        super(props);
        this.state = { initializing: true, loading: false };
        fetch('api/PdfGenerator/GetLegendaryExpansions')
            .then(response =>
                response.json())
            .then(data => {
                this.setState({ expansions: data, initializing: false });
            });
    }

    render() {
        let contents = null;
        if (this.state.initializing) {
            contents = <p><em>Initializing...</em></p>
        }
        else {
            contents =
            <form method='post' action='api/PdfGenerator/GenerateLegendary'>
                <select name='expansionNames' className='selectmultiple form-control' defaultValue={['Legendary']} multiple='multiple' size={this.state.expansions.length}>
                    {this.state.expansions.map((expansion => <option key={expansion}>{expansion}</option>))}
                </select>
                <div className='form-group'>
                    <div className='checkbox'>
                        <label htmlFor='includeSpecialSetupCards'>
                            <input className='checkboxinput' type='checkbox' name='includeSpecialSetupCards' id='includeSpecialSetupCards' defaultChecked={false} value='true'/>
                            Include dividers for special setup cards (Special Bystanders, Special Wounds, etc)
                        </label>
                    </div>
                </div>
    <input type='submit' className='btn btn-primary' value='Generate Labels'></input>
    </form>
}

return (
    <div>
    <h1>Legendary</h1>
    {contents}
    </div>
);
}
}
