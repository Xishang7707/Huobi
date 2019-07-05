/**
 * 
 * HuobiWebSocke操作对象
 * 
 * */
function HuobiSocket(url) {

    let m_url = url;
    let m_socket;

    let m_send_list = {};
    let m_on_list = {};

    /**
     * 封装数据格式
     * @param {string} target
     * @param {string} method
     * @param {any} data
     */
    function make_data(target, method, data = {}) {
        return {
            target: target,
            method: method,
            data: data
        };
    }

    /**
     * 消息处理
     * @param {string} target  目标
     * @param {string} method  方法
     * @param {any} data    数据
     */
    function message_proc(target, method, data) {
        if (method == "send") {
            if (!m_send_list["send_" + target])
                return;
            m_send_list["send_" + target](data);
        } else if (method == "on") {
            if (!m_on_list['on_' + target] || !m_on_list['on_' + target][0])
                return;
            m_on_list['on_' + target][0](data);
        }
    }

    /**
     * 连接
     * */
    this.connect = function () {
        m_socket = new WebSocket(m_url);

        m_socket.onopen = function (e) {//已连接
            for (var key in m_on_list)
                m_socket.send(JSON.stringify(m_on_list[key][1]));
        }

        //消息监听
        m_socket.onmessage = function (e) {
            let data = JSON.parse(e['data']);
            message_proc(data["target"], data["method"], data["data"]);
        }
    }

    /**
     * 关闭
     * */
    this.close = function (e) {
        m_socket.close();
        if (e)
            e();
    }

    /**
     * 监听
     * @param {any} target  监听对象
     * @param {any} call    监听回调
     */
    this.on = function (target, call) {
        let send_data = make_data(target, "on");

        m_on_list["on_" + target] = [call, send_data];

        if (m_socket.readyState == 1)//已连接
            m_socket.send(JSON.stringify(send_data));
    }

    /**
     * 发送数据
     * @param {string} target  目标
     * @param {any} data    数据
     * @param {Function} call    结果
     */
    this.send = function (target, data, call) {
        let send_data = make_data(target, "send", data);


        m_socket.send(JSON.stringify(send_data));

        m_send_list["send_" + target] = call;
    }
}