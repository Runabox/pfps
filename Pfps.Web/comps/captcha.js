import ReCAPTCHA from "react-google-recaptcha";
import React from 'react';

class Captcha extends React.Component {
    constructor(props) {
        super(props);
    }
    
    render() {
        return (
            <ReCAPTCHA
                sitekey="6LctNQIgAAAAAAUHwRD3MBmB0GRnLAWvu-6MeSCp"
                onChange={(res) => this.props.response(res)}
            />
        );
    }
}

export default Captcha;